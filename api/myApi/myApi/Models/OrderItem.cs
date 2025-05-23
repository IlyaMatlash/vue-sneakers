﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace myApi.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
