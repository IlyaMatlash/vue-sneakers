﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace myApi.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? RecipientName { get; set; }
        
        [Required]
        [StringLength(200)]
        public string? RecipientAddress { get; set; }
        
        [Required]
        [StringLength(20)]
        public string? PostalCode { get; set; }
        
        [Required]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(50)]
        public string? PromoCode { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? DeliveryMethod { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string? Status { get; set; }
        
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
