using OnlineBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineBookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookStoreEntities _context;

        public HomeController()
        {
            _context = new BookStoreEntities();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Types()
        {
            ViewBag.Message = "Your Types Page";
            return View();
        }
        public ActionResult Category()
        {
            var books = _context.Books.ToList();
            return View(books);
        }
      
    }
}