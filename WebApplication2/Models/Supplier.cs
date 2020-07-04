using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("Supplier")]
    public class Supplier
    {

        public Address address { get; set; } = new Address();


        [ForeignKey("User")]
        [Key]
        public string UserID { get; set; }

        public virtual IdentityUser User { get; set; }
        public virtual ICollection<Product> Products { get; set; }



    }
}