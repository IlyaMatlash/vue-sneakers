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

namespace myApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FavoriteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get([FromQuery] string sessionId)
        {
            string query = @"
                            SELECT f.FavoriteId, f.SessionId, f.ProductId 
                            FROM dbo.Favorites f
                            WHERE f.SessionId = @SessionId";

            DataTable table = new DataTable();
             string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Favorite favorite)
        {
            try
    {
        string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection myCon = new SqlConnection(sqlDataSource))
        {
            myCon.Open();
            
            // Проверяем существование продукта
            string productQuery = @"SELECT COUNT(1) FROM dbo.Products WHERE ProductId = @ProductId";
            using (SqlCommand productCommand = new SqlCommand(productQuery, myCon))
            {
                productCommand.Parameters.AddWithValue("@ProductId", favorite.ProductId);
                int productExists = (int)productCommand.ExecuteScalar();
                if (productExists == 0)
                {
                    return new JsonResult(new { error = "Продукт не найден" }) { StatusCode = 404 };
                }
            }
            // Проверяем дубликаты
            string duplicateQuery = @"SELECT COUNT(1) FROM dbo.Favorites 
                                    WHERE SessionId = @SessionId AND ProductId = @ProductId";
            using (SqlCommand duplicateCommand = new SqlCommand(duplicateQuery, myCon))
            {
                duplicateCommand.Parameters.AddWithValue("@SessionId", favorite.SessionId);
                duplicateCommand.Parameters.AddWithValue("@ProductId", favorite.ProductId);
                int duplicateExists = (int)duplicateCommand.ExecuteScalar();
                if (duplicateExists > 0)
                {
                    return new JsonResult(new { error = "Товар уже в избранном" }) { StatusCode = 400 };
                }
            }
            // Добавляем в избранное
            string insertQuery = @"INSERT INTO dbo.Favorites (SessionId, ProductId)
                                VALUES (@SessionId, @ProductId);
                                SELECT SCOPE_IDENTITY();";
            using (SqlCommand insertCommand = new SqlCommand(insertQuery, myCon))
            {
                insertCommand.Parameters.AddWithValue("@SessionId", favorite.SessionId);
                insertCommand.Parameters.AddWithValue("@ProductId", favorite.ProductId);
                int newId = Convert.ToInt32(insertCommand.ExecuteScalar());
                return new JsonResult(new { id = newId, message = "Товар успешно добавлен в избранное" });
            }
        }
    }
    catch (Exception ex)
    {
        return new JsonResult(new { error = "Ошибка: " + ex.Message }) { StatusCode = 500 };
    }
        }

        [HttpPut]
        public JsonResult Put(Favorite favorite)
        {
            string query = @"
                           update dbo.Favorites
                           set SessionId= @SessionId, ProductId= @ProductId
                            where FavoriteId=@FavoriteId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@FavoriteId", favorite.FavoriteId);
                    myCommand.Parameters.AddWithValue("@SessionId", favorite.SessionId);
                    myCommand.Parameters.AddWithValue("@ProductId", favorite.ProductId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Избранное обновлено");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                           delete from dbo.Favorites
                            where FavoriteId=@FavoriteId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@FavoriteId", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Избранное удалено");
        }
    }
}