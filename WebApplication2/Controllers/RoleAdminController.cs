using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{

   // [Authorize(Roles = "Admin")]
    
   
    public class RoleAdminController : Controller
    {
        // GET: RoleAdmin
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(string NameRole)
        {
            
                ApplicationDbContext context = new ApplicationDbContext();
                RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);
                RoleManager<IdentityRole> Manager = new RoleManager<IdentityRole>(roleStore);
                IdentityRole role = new IdentityRole();
                role.Name = NameRole;
                Manager.Create(role);
                return RedirectToAction("Index", "Home");                    
        }
    }
}