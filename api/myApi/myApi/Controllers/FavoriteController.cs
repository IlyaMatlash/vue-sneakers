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
        public JsonResult Get()
        {
            string query = @"
                            select FavoriteId, UserId, ProductId from
                            dbo.Favorites
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
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
            
            // Если пользователя нет, создаём гостевого пользователя
            string checkUserQuery = @"SELECT COUNT(1) FROM dbo.Users WHERE UserId = @UserId";
            using (SqlCommand cmd = new SqlCommand(checkUserQuery, myCon))
            {
                cmd.Parameters.AddWithValue("@UserId", favorite.UserId);
                int userExists = (int)cmd.ExecuteScalar();
                if (userExists == 0)
                {
                    // Создаем нового гостевого пользователя
                    string insertUserQuery = @"
                    INSERT INTO dbo.Users (FirstName, LastName, Patronymic, Email, Password, Role)
                    VALUES (@FirstName, @LastName, @Patronymic, @Email, @Password, @Role);
                    SELECT SCOPE_IDENTITY();";
                    using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, myCon))
                    {
                        insertCmd.Parameters.AddWithValue("@FirstName", "Guest");
                        insertCmd.Parameters.AddWithValue("@LastName", "Guest");
                        insertCmd.Parameters.AddWithValue("@Patronymic", "Guest");
                        insertCmd.Parameters.AddWithValue("@Email", "Guest");
                        insertCmd.Parameters.AddWithValue("@Password", "Guest");
                        insertCmd.Parameters.AddWithValue("@Role", "Guest");
                        int newUserId = Convert.ToInt32(insertCmd.ExecuteScalar());
                        favorite.UserId = newUserId;
                    }
                }
            }
            
            // Проверяем существование продукта (аналогично предыдущему примеру)
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
            // Проверяем дубликаты и добавляем запись в избранное (как в предыдущем примере)
            string duplicateQuery = @"SELECT COUNT(1) FROM dbo.Favorites 
                                    WHERE UserId = @UserId AND ProductId = @ProductId";
            using (SqlCommand duplicateCommand = new SqlCommand(duplicateQuery, myCon))
            {
                duplicateCommand.Parameters.AddWithValue("@UserId", favorite.UserId);
                duplicateCommand.Parameters.AddWithValue("@ProductId", favorite.ProductId);
                int duplicateExists = (int)duplicateCommand.ExecuteScalar();
                if (duplicateExists > 0)
                {
                    return new JsonResult(new { error = "Товар уже в избранном" }) { StatusCode = 400 };
                }
            }
            string insertQuery = @"INSERT INTO dbo.Favorites (UserId, ProductId)
                                 VALUES (@UserId, @ProductId);
                                 SELECT SCOPE_IDENTITY();";
            using (SqlCommand insertCommand = new SqlCommand(insertQuery, myCon))
            {
                insertCommand.Parameters.AddWithValue("@UserId", favorite.UserId);
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
                           set UserId= @UserId, ProductId= @ProductId
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
                    myCommand.Parameters.AddWithValue("@UserId", favorite.UserId);
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