using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier supplier { get; set; }

        [Display(Name ="Category Name")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category category { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public virtual ICollection<OrderDetails> orderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }


    }
}