using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using myApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace myApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;
        private const string ProductsCacheKey = "ProductsList";
        public ProductController(IConfiguration configuration, IWebHostEnvironment env, IMemoryCache cache)
        {
            _configuration = configuration;
            _env = env;
            _cache = cache;
        }

        [HttpGet]
        public async Task<JsonResult> Get([FromQuery] Dictionary<string, string> filters)
        {
            // Попытка получить данные из кэша
            string cacheKey = GetCacheKey(filters);
            if (_cache.TryGetValue(cacheKey, out DataTable cachedTable))
            {
                return new JsonResult(cachedTable);
            }
            string query = BuildFilterQuery(filters);
            var parameters = CreateSqlParameters(filters);
            DataTable table = await ExecuteQueryAsync(query, parameters);
            // Кэширование результатов на 5 минут
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, table, cacheOptions);
            return new JsonResult(table);
        }
        private string GetCacheKey(Dictionary<string, string> filters)
        {
            if (filters == null || filters.Count == 0)
                return ProductsCacheKey;
            return $"{ProductsCacheKey}_{string.Join("_", filters.OrderBy(f => f.Key).Select(f => $"{f.Key}={f.Value}"))}";
        }
        private string BuildFilterQuery(Dictionary<string, string> filters)
        {
            var query = @"
                SELECT DISTINCT p.* 
                FROM dbo.Products p
                WHERE 1=1";
            if (filters != null)
            {
                if (filters.ContainsKey("priceMin") && decimal.TryParse(filters["priceMin"], out decimal minPrice))
                    query += " AND p.Price >= @PriceMin";
            
                if (filters.ContainsKey("priceMax") && decimal.TryParse(filters["priceMax"], out decimal maxPrice))
                    query += " AND p.Price <= @PriceMax";
                    
                if (filters.ContainsKey("brands"))
                    query += " AND p.Brand IN (SELECT value FROM STRING_SPLIT(@Brands, ','))";
                    
                if (filters.ContainsKey("sizes"))
                    query += " AND EXISTS (SELECT 1 FROM STRING_SPLIT(p.Sizes, ',') s WHERE TRIM(s.value) IN (SELECT TRIM(value) FROM STRING_SPLIT(@Sizes, ',')))";
                    
                if (filters.ContainsKey("categories"))
                    query += " AND p.Category IN (SELECT value FROM STRING_SPLIT(@Categories, ','))";
                    
                if (filters.ContainsKey("seasons"))
                    query += " AND p.Season IN (SELECT value FROM STRING_SPLIT(@Seasons, ','))";
                    
                if (filters.ContainsKey("colors"))
                    query += " AND p.Color IN (SELECT value FROM STRING_SPLIT(@Colors, ','))";
                    
                if (filters.ContainsKey("materials"))
                    query += " AND p.Material IN (SELECT value FROM STRING_SPLIT(@Materials, ','))";
                    
                if (filters.ContainsKey("features"))
                    query += " AND p.Features LIKE '%' + @Features + '%'";
            }
            return query;
        }
        private List<SqlParameter> CreateSqlParameters(Dictionary<string, string> filters)
        {
            var parameters = new List<SqlParameter>();
            
            if (filters != null)
            {
                if (filters.ContainsKey("priceMin") && decimal.TryParse(filters["priceMin"], out decimal minPrice))
                    parameters.Add(new SqlParameter("@PriceMin", minPrice));
            
                if (filters.ContainsKey("priceMax") && decimal.TryParse(filters["priceMax"], out decimal maxPrice))
                    parameters.Add(new SqlParameter("@PriceMax", maxPrice));
                    
                if (filters.ContainsKey("brands"))
                    parameters.Add(new SqlParameter("@Brands", filters["brands"]));
                    
                if (filters.ContainsKey("sizes"))
                    parameters.Add(new SqlParameter("@Sizes", filters["sizes"]));
                    
                if (filters.ContainsKey("categories"))
                    parameters.Add(new SqlParameter("@Categories", filters["categories"]));
                    
                if (filters.ContainsKey("seasons"))
                    parameters.Add(new SqlParameter("@Seasons", filters["seasons"]));
                    
                if (filters.ContainsKey("colors"))
                    parameters.Add(new SqlParameter("@Colors", filters["colors"]));
                    
                if (filters.ContainsKey("materials"))
                    parameters.Add(new SqlParameter("@Materials", filters["materials"]));
                    
                if (filters.ContainsKey("features"))
                    parameters.Add(new SqlParameter("@Features", filters["features"]));
            }
            return parameters;
        }
        private async Task<DataTable> ExecuteQueryAsync(string query, List<SqlParameter> parameters)
        {
            var table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    if (parameters != null)
                        parameters.ForEach(p => myCommand.Parameters.Add(p));
                    using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                    {
                        table.Load(myReader);
                    }
                }
            }
            return table;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            try
            {
                if (product == null)
                    return BadRequest("Product data is null");
                // Validate required fields
                var validationContext = new ValidationContext(product);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(product, validationContext, validationResults, true))
                {
                    return BadRequest(validationResults.Select(r => r.ErrorMessage));
                }
                // Initialize collections if null
                product.Images ??= new List<string>();
                product.Sizes ??= new List<string>();
                string query = @"
                    INSERT INTO dbo.Products 
                    (Name, Description, Price, Images, Brand, Sizes, Category, Season, Color, Material, Features)
                    VALUES 
                    (@Name, @Description, @Price, @Images, @Brand, @Sizes, @Category, @Season, @Color, @Material, @Features);
                    SELECT SCOPE_IDENTITY();";
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", product.Name);
                        myCommand.Parameters.AddWithValue("@Description", product.Description ?? "");
                        myCommand.Parameters.AddWithValue("@Price", product.Price);
                        myCommand.Parameters.AddWithValue("@Images", string.Join(",", product.Images));
                        myCommand.Parameters.AddWithValue("@Brand", product.Brand ?? "");
                        myCommand.Parameters.AddWithValue("@Sizes", string.Join(",", product.Sizes));
                        myCommand.Parameters.AddWithValue("@Category", product.Category ?? "");
                        myCommand.Parameters.AddWithValue("@Season", product.Season ?? "");
                        myCommand.Parameters.AddWithValue("@Color", product.Color ?? "");
                        myCommand.Parameters.AddWithValue("@Material", product.Material ?? "");
                        myCommand.Parameters.AddWithValue("@Features", product.Features ?? "");
                        int newId = Convert.ToInt32(await myCommand.ExecuteScalarAsync());
                        _cache.Remove(ProductsCacheKey);
                        
                        return Ok(new { id = newId, message = "Продукт успешно добавлен" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Product product)
        {
            try
            {
                if (product == null)
                    return BadRequest("Product data is null");
                if (string.IsNullOrEmpty(product.Name))
                    return BadRequest("Product name is required");
                    
                if (product.Price <= 0)
                    return BadRequest("Price must be greater than 0");
                product.Images ??= new List<string>();
                product.Sizes ??= new List<string>();
                string query = @"
                    UPDATE dbo.Products
                    SET Name = @Name,
                        Description = @Description,
                        Price = @Price,
                        Images = @Images,
                        Brand = @Brand,
                        Sizes = @Sizes,
                        Category = @Category,
                        Season = @Season,
                        Color = @Color,
                        Material = @Material,
                        Features = @Features
                    WHERE ProductId = @ProductId";
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@ProductId", product.ProductId);
                        myCommand.Parameters.AddWithValue("@Name", product.Name);
                        myCommand.Parameters.AddWithValue("@Description", product.Description ?? "");
                        myCommand.Parameters.AddWithValue("@Price", product.Price);
                        myCommand.Parameters.AddWithValue("@Images", string.Join(",", product.Images));
                        myCommand.Parameters.AddWithValue("@Brand", product.Brand ?? "");
                        myCommand.Parameters.AddWithValue("@Sizes", string.Join(",", product.Sizes));
                        myCommand.Parameters.AddWithValue("@Category", product.Category ?? "");
                        myCommand.Parameters.AddWithValue("@Season", product.Season ?? "");
                        myCommand.Parameters.AddWithValue("@Color", product.Color ?? "");
                        myCommand.Parameters.AddWithValue("@Material", product.Material ?? "");
                        myCommand.Parameters.AddWithValue("@Features", product.Features ?? "");
                        int newId = Convert.ToInt32(await myCommand.ExecuteScalarAsync());
                        _cache.Remove(ProductsCacheKey);

                        return Ok(new { id = newId, message = "Продукт успешно добавлен" });
                    }
                }                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                           delete from dbo.Products
                            where ProductId=@ProductId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductId", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Продукт удален");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/sneakers/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("Картинка не сохранилась");
            }
        }
    }
}