using System.ComponentModel.DataAnnotations;

namespace myApi.Models
{
    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public string? Name { get; set; }
    }
}