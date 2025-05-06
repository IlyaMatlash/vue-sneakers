using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace myApi.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
