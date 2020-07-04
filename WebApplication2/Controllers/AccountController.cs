using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebApplication2.Models;
using WebApplication2.ViewModel;

namespace WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Account
        public ActionResult Index()
        {          
           return View();          
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
       [HttpPost]
        public ActionResult Login(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                UserStore<IdentityUser> store = new UserStore<IdentityUser>(context);
                UserManager<IdentityUser> manger = new UserManager<IdentityUser>(store);
                IdentityUser identityUser = manger.FindByName(user.UserName);
                bool Found = manger.CheckPassword(identityUser, user.Password);
                if (Found)
                {
                    string id = (from U in context.Users
                                 where U.UserName == user.UserName
                                 select U.Id).ToString();

                    //create cookie
                    SignInManager<IdentityUser, string> signInManager = new SignInManager<IdentityUser, string>(manger, HttpContext.GetOwinContext().Authentication);
                    signInManager.SignIn(identityUser, true, false);
                    Session["ID"] = identityUser.Id;
                    if (manger.IsInRole(identityUser.Id, "Suplier") || manger.IsInRole(identityUser.Id, "Admin"))
                    {

                        //Session["Id"] = identityUser.Id;
                        //idcookie = name cookie
                        HttpCookie supplierCookie = new HttpCookie("idcookie");// "Id", identityUser.Id);


                        supplierCookie.Value = identityUser.Id;
                        DateTime dd = DateTime.Now.AddDays(2);
                        supplierCookie.Expires = dd;
                        Response.Cookies.Add(supplierCookie);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "user Name or Password not correct");
                }
            }

            return View();

        }
        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(Register register, HttpPostedFileBase file)

        {
            if(ModelState.IsValid)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                Customer customer = new Customer();
                UserStore<IdentityUser> store = new UserStore<IdentityUser>(context);
                UserManager<IdentityUser> manger = new UserManager<IdentityUser>(store);
                IdentityUser identityUser = new IdentityUser();
                identityUser.UserName = register.UserName;
                identityUser.Email = register.Email;
               
                IdentityResult result = manger.Create(identityUser, register.Password);
                if(result.Succeeded)
                {
                     //manger.AddToRole(identityUser.Id, "Admin");
                    //manger.AddToRole(identityUser.Id, "Suplier");
                    manger.AddToRole(identityUser.Id, "Customer");
                    SignInManager<IdentityUser, string> signInManager = 
                        new SignInManager<IdentityUser, string>(manger, HttpContext.GetOwinContext().Authentication);
                    signInManager.SignIn(identityUser, true, false);
                    customer.UserID = identityUser.Id;
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        customer.Image = fileName;
                        file.SaveAs(path);
                    }

                    customer.address.Country = register.Country;
                    customer.address.City = register.City;
                    customer.address.Street = register.Street;
                    //customer.User.EmailConfirmed = false;
                    context.Customers.Add(customer);
                    context.SaveChanges();
                    BuildEmailTemplate(customer.UserID);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item);
                    }
                }
            }
            return View(register);
        }

        public ActionResult Confirm(string regId)
        {
            ViewBag.regID = regId;
            return View();
        }
        public ActionResult RegisterConfirm(string regId)
        {
            Customer Data = context.Customers.Where(x => x.UserID == regId).FirstOrDefault();
            //Data.IsActivated = true;
            Data.User.EmailConfirmed = true;
            context.SaveChanges();
            //var msg = "Your Email is Verified!";
            return RedirectToAction("Login", "Account");
            //return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public void BuildEmailTemplate(string regID)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var regInfo = context.Customers.Where(x => x.UserID == regID).FirstOrDefault();
            var url = "http://localhost:57023/" + "Account/Confirm?regId=" + regID;
            body = body.Replace("ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account is Successfully Created!", body, regInfo.User.Email);
        }

        public void BuildEmailTemplate(string subjectText, string bodyText, string SendTo)
        {
            string from, to, cc, bcc, subject, body;
            from = "ahmedammar144430@gmail.com";
            to = SendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.Bcc.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("ahmedammar144430@gmail.com", "kylewalker2019");
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult RegisterSuplier()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterSuplier(Register register)
        {
            Supplier supplier = new Supplier();
            if (ModelState.IsValid)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                UserStore<IdentityUser> store = new UserStore<IdentityUser>(context);
                UserManager<IdentityUser> manger = new UserManager<IdentityUser>(store);
                IdentityUser identityUser = new IdentityUser();
                identityUser.UserName = register.UserName;
                identityUser.Email = register.Email;
                IdentityResult result = manger.Create(identityUser, register.Password);
                if (result.Succeeded)
                {
                   
                    manger.AddToRole(identityUser.Id, "Suplier");
                   
                    SignInManager<IdentityUser, string> signInManager =
                        new SignInManager<IdentityUser, string>(manger, HttpContext.GetOwinContext().Authentication);
                    signInManager.SignIn(identityUser, true, false);
                    supplier.UserID = identityUser.Id;
                    supplier.address.Country = register.Country;
                    supplier.address.City = register.City;
                    supplier.address.Street = register.Street;
                    context.Suppliers.Add(supplier);
                    context.SaveChanges();
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item);
                    }
                }
            }
            return View(register);
        }
        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.
                SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
       
       
    }
}