using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using WebApplication2.Models;

using Microsoft.AspNet.SignalR;
using WebApplication2.Hubs;
using System.Web.Mvc;
namespace WebApplication2.Controllers
{
    
    public class AdminSupplierController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: AdminSupplier
      
        [System.Web.Mvc.Authorize(Roles = "Suplier,Admin")]
        public ActionResult Index()
        {
            List<Product> prod = context.Products.ToList();
            return View(prod);
        }
     
        [System.Web.Mvc.Authorize(Roles = "Suplier")]
        [HttpGet]
        public ActionResult AddProduct()
        {
            List<Category> category = context.Categories.ToList();
            ViewBag.category = category;
            return View();
        }
        [HttpPost]
       
        [System.Web.Mvc.Authorize(Roles = "Suplier")]
        public ActionResult AddNewProduct(Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid == true)
            {
                Product pro = new Product();
                pro.Name = product.Name;
                pro.Price = product.Price;
                pro.Description = product.Description;
                pro.CategoryId = product.CategoryId;

                // pro.SupplierId =Session["Id"].ToString();
                HttpCookie cookie = Request.Cookies["idcookie"];
                pro.SupplierId = cookie.Value.ToString();
                pro.Stock = product.Stock;
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    pro.Image = fileName;
                    file.SaveAs(path);
                }
                context.Products.Add(pro);
                context.SaveChanges();
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<AddNewProduct>();

                hubContext.Clients.All.AddNewDepartmentNow(product);

                return RedirectToAction("Index");

            }
            else
            {
                List<Category> category = context.Categories.ToList();
                ViewBag.category = category;
                return View("AddProduct");
            }

        }
        [HttpGet]
      
        [System.Web.Mvc.Authorize(Roles = "Suplier")]
        public ActionResult Edit(int id)
        {
            List<Category> category = context.Categories.ToList();
            ViewBag.category = category;
            Product prod = context.Products.FirstOrDefault(s => s.Id == id);
            return View(prod);
        }
        [System.Web.Mvc.Authorize(Roles = "Suplier")]
        [HttpPost]
        public ActionResult Edit(int id,Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid == true)
            {
                Product pro = context.Products.FirstOrDefault(s => s.Id == id);
                pro.Name = product.Name;
                pro.Price = product.Price;
                pro.Description = product.Description;
                pro.CategoryId = product.CategoryId;

                // pro.SupplierId =Session["Id"].ToString();
                HttpCookie cookie = Request.Cookies["idcookie"];
                pro.SupplierId = cookie.Value.ToString();
                pro.Stock = product.Stock;
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    pro.Image = fileName;
                    file.SaveAs(path);
                }
                
                context.SaveChanges();
              

                return RedirectToAction("Index");

            }
            else
            {
               
                return View("AddProduct");
            }


        }
        [System.Web.Mvc.Authorize(Roles = "Suplier,Admin")]
        public ActionResult Delete(int id)
        {
            Product prod = context.Products.FirstOrDefault(s => s.Id == id);
            context.Products.Remove(prod);
            context.SaveChanges();
            return RedirectToAction("Index");

        }
    }
    }