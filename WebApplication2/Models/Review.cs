using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("Review")]
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product product { get; set; }
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer customer { get; set; }
        public string ContentReview { get; set; }
        public string CustomerImage { get; set; }

        public string CustomerName { get; set; }
        public double Rate { get; set; }
        public DateTime Date { get; set; }
    }
}