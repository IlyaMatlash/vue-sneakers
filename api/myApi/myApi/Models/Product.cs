using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace myApi.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(200)]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Описание товара обязательно")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Цена товара обязательна")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public List<string> Images { get; set; }
        [Required(ErrorMessage = "Бренд обязателен")]
        public string? Brand { get; set; }
        [Required(ErrorMessage = "Размеры обязательны")]
        public List<string> Sizes { get; set; }
        [Required(ErrorMessage = "Категория обязательна")]
        public string? Category { get; set; }
        public string? Season { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public string? Features { get; set; }
        public Product()
        {
            Images = new List<string>();
            Sizes = new List<string>();
        }
    }
}
