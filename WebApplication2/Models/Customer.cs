using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("Customer")]
    public class Customer
    {
        public Address address { get; set; } = new Address();
        public string Image { get; set; }
        [ForeignKey("User")]
        [Key]    
        public string UserID { get; set; }
        public virtual IdentityUser User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}