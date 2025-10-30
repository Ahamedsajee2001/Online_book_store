using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OnlineBookStore.Models;

namespace OnlineBookStore.Controllers
{
   
    public class AccountController : Controller
    {
        BookStoreEntities db = new BookStoreEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer cus)
        {
            if (ModelState.IsValid)
            {
                if (db.Customers.Any(x => x.Email == cus.Email))
                {
                    ViewBag.Message = "Email already registered";
                }
                else
                {
                    db.Customers.Add(cus);
                    db.SaveChanges();
                    ViewBag.Success = true; // Flag indicating success
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(CusLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Customers.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // Set the authentication cookie
                    FormsAuthentication.SetAuthCookie(user.Email, false);

                    // Store the username and user ID in the session
                    Session["Username"] = user.Cus_name;
                    Session["UserId"] = user.Cus_id;

                    // Initialize cart item count
                    Session["CartItemCount"] = db.CartItems.Where(c => c.Cus_id_fk == user.Cus_id).Sum(c => c.Quantity);

                    // Redirect to Category page
                    return RedirectToAction("Category", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Attempt.");
                }
            }

            return View(model);
        }


        public ActionResult Logout()
        {
            // Sign out the user
            FormsAuthentication.SignOut();

            // Clear the authentication session
            Session.Clear();

            // Redirect to the Login page
            return RedirectToAction("Login", "Account");
        }

        private bool ValidateUser(string email, string password)
        {
            // Check the user in the database
            var user = db.Customers.FirstOrDefault(u => u.Email == email && u.Password == password);
            return user != null;
        }

    }
}