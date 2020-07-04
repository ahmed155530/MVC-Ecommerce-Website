using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    [Table("Order")]
    public class Order
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [Column(TypeName ="date")]
        public DateTime OrderDate{ get; set; }
      
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer customer { get; set; }
        public virtual ICollection<OrderDetails> orderDetails { get; set; }

        
    }
}