using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace myApi.Models
{
    public class OrderDto
    {
        public List<OrderItemDto>? OrderItems { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        [Required]
        public string? RecipientName { get; set; }
        
        [Required]
        public string? RecipientAddress { get; set; }
        
        [Required]
        public string? PostalCode { get; set; }
        
        [Required]
        public string? PhoneNumber { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? PromoCode { get; set; }
        
        [Required]
        public string? DeliveryMethod { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public string? Status { get; set; }
    }
}