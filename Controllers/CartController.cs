using OnlineBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineBookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly BookStoreEntities db = new BookStoreEntities();
        // GET: Cart
        public ActionResult Index()
        {
            int userId = (int)Session["UserId"];
            var cartItems = db.CartItems.Where(c => c.Cus_id_fk == userId).ToList();
            return View(cartItems);
        }
        [HttpPost]
        public ActionResult AddToCart(int bookId, int quantity)
        {
            int userId = (int)Session["UserId"];

            var cartItem = db.CartItems.FirstOrDefault(c => c.BookId_fk == bookId && c.Cus_id_fk == userId);
            if(cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    BookId_fk = bookId,
                    Cus_id_fk = userId,
                    Quantity = quantity
                };
                db.CartItems.Add(cartItem);
            }
            db.SaveChanges();

            //Update the session cart item count

            Session["CartItemCount"] = db.CartItems.Where(c => c.Cus_id_fk == userId).Sum(c => c.Quantity);
            return RedirectToAction("Index");
        }
    }
}