using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Item
    {
        public Product product { get; set; } = new Product();
        public int Quantity { get; set; }

    }
}