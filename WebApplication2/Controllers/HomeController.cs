using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Hubs;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        public ActionResult Index()
        {
            List<Product> product = context.Products.Where(p => p.Id <= 6).ToList();
      
            return View(product);
        }
        public ActionResult ShowAllProduct()
        {
            return PartialView("_ShowAllProducts",context.Products.Where(p => p.Id > 6).ToList());
        }
        public ActionResult ShowAllProduct2()
        {
            return PartialView("_ShowAllProducts", context.Products.ToList());
        }


        public ActionResult Search(string Search, string selectCategory)
        {
            //List<Product> products = context.Products.Where(c => c.Name.Contains(Search)).ToList();
            
            if (Search != "" && selectCategory != null && selectCategory != "All Categories")
            {
            List<Product> products = (from p in context.Products
                                        from k in context.Categories
                                        where p.Name.Contains(Search)
                                        where k.Name == selectCategory
                                      where p.CategoryId == k.Id
                                        select p).ToList();
            return View(products);
            }
            if (Search != "" && selectCategory == "All Categories")
            {

                List<Product> products = context.Products.Where(p => p.Name.Contains(Search)).ToList();
                return View(products);
            }
            else if (Search == "" && selectCategory != null && selectCategory != "All Categories")
            {
                List<Product> products = (from p in context.Products
                                          from k in context.Categories                                         
                                          where k.Name == selectCategory
                                          where k.Id==p.CategoryId
                                          select p).ToList();
                return View(products); 
            }
            else if (Search == "" && selectCategory == "All Categories")
            {
                List<Product> products = context.Products.ToList();
                return View(products);
            }
            else if (Search != "" && selectCategory == "All Categories")
            {
                List<Product> products = context.Products.Where(p => p.Name.Contains(Search)).ToList();
                return View(products);
            }
            else if(Search != null && selectCategory == null)
            {
                List<Product> products = context.Products.Where(p => p.Name.Contains(Search)).ToList();
                return View(products);
            }
            else
            {

                return RedirectToAction("Index");
            }

        }
        [System.Web.Mvc.Authorize(Roles ="Customer")]
        public ActionResult ProductDetails(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            List<Product> prod = (from produc in context.Products
                                  from catogry in context.Categories
                                  where produc.CategoryId == catogry.Id
                                  select produc).ToList();
            ViewBag.name = prod;

            return View(product);
        }
        
        public ActionResult  WiteReview()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendReview(Review review)
        {
            string CustomerID = Session["ID"].ToString();
            
            Customer customer = context.Customers.FirstOrDefault(c => c.UserID == CustomerID);
            string CustomerName = (from U in context.Users where U.Id == CustomerID select U.UserName).FirstOrDefault();
            Review ReviewCustomer = new Review();
            ReviewCustomer.ProductId = review.ProductId;
            ReviewCustomer.CustomerId = customer.UserID;
            ReviewCustomer.CustomerImage = customer.Image;
            ReviewCustomer.CustomerName = CustomerName;

            ReviewCustomer.Rate = review.Rate;
            ReviewCustomer.ContentReview = review.ContentReview;
            ReviewCustomer.Date = DateTime.Now;
            context.Reviews.Add(ReviewCustomer);
            context.SaveChanges();
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Reviews>();
            hubContext.Clients.All.NewReview(review);
            return RedirectToAction("ProductDetails", "Home", new { id = review.ProductId });
        }
        public ActionResult Buy(int Id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == Id);
            if (Session["cart"] == null)
            {
                if(product.Stock>=1)
                {
                    List<Models.Item> cart = new List<Models.Item>();
                    cart.Add(new Models.Item { product = context.Products.FirstOrDefault(p => p.Id == Id), Quantity = 1 });
                    Session["cart"] = cart;
                }               
            }
            else
            {
                List<Models.Item> cart = (List<Models.Item>)Session["cart"];

                int index = isExist(Id);
                
                if (index != -1)
                {
                    if(product.Stock > cart[index].Quantity)
                    {
                        cart[index].Quantity++;
                    }                                    
                }
                else
                {
                    if (product.Stock >= 1)
                    {
                        cart.Add(new Models.Item { product = context.Products.FirstOrDefault(p => p.Id == Id), Quantity = 1 });
                    }
                }
                Session["cart"] = cart;
            }
            return View();
        }

        public ActionResult Remove(int Id)
        {
            List<Models.Item> cart = (List<Models.Item>)Session["cart"];
            int index = isExist(Id);
            var query = cart[index];
            cart.Remove(query);
            Session["cart"] = cart;
            return View("Buy");
        }
     
        private int isExist(int Id)
        {
            List<Models.Item> cart = (List<Models.Item>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].product.Id.Equals(Id))
                    return i;
            return -1;
        }

       public ActionResult Update(FormCollection Fc)
        {
            string[] Quantities = Fc.GetValues("Quantity");
            List<Models.Item> cart = (List<Models.Item>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                cart[i].Quantity = Convert.ToInt32(Quantities[i]);

            }
            Session["cart"] = cart;
            return View("Buy");
        }
        public ActionResult showCart()
        {
            List<Models.Item> cart = (List<Models.Item>)Session["cart"];
            return View(cart);
        }
        public ActionResult CheckOut()
        {
            List<Models.Item> cart = (List<Models.Item>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                var item = cart[i];
                var product = context.Products.FirstOrDefault(p => p.Id == item.product.Id);
                product.Stock = product.Stock - item.Quantity;
              
                
            }
            context.SaveChanges();
         

              // Session["cart"] = null;

            return View();

        }

      
    }
}