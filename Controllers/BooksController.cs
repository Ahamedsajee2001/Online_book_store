using OnlineBookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineBookStore.Controllers
{
    public class BooksController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();
        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Admin);
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.admin_id_fk = new SelectList(db.Admins, "Admin_id", "Username");
            ViewBag.Category_id_fk = new SelectList(db.Books_Category, "Category_id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Book_id,Category_id_fk,year,Price,Image,Author,Title,Publication_Year,Pages,ISBN,Language,admin_id_fk")] Book book, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if an image file was uploaded
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Convert the image to Base64
                        using (MemoryStream ms = new MemoryStream())
                        {
                            imageFile.InputStream.CopyTo(ms);
                            book.Image = ms.ToArray(); // Assign the Base64 byte array to the Image property
                        }
                    }

                    // Add the book to the database
                    db.Books.Add(book);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating book: " + ex.Message);
                }
            }

            // If model state is not valid, repopulate dropdown lists and return the view with errors
            PopulateDropdownLists(book);
            return View(book);
        }

        private void PopulateDropdownLists(Book book)
        {
            ViewBag.admin_id_fk = new SelectList(db.Admins, "Admin_id", "Username", book.admin_id_fk);
            ViewBag.Category_id_fk = new SelectList(db.Books_Category, "Category_id", "Name", book.Category_id_fk);
        }




        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.admin_id_fk = new SelectList(db.Admins, "Admin_id", "Username", book.admin_id_fk);
            ViewBag.Category_id_fk = new SelectList(db.Books_Category, "Category_id", "Name", book.Category_id_fk);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_id,Category_id_fk,year,Price,Image,Author,Title,Publication_Year,Pages,ISBN,Language,admin_id_fk")] Book book, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            imageFile.InputStream.CopyTo(ms);
                            book.Image = ms.ToArray();
                        }
                    }
                    else
                    {
                        // Preserve existing image if no new image is uploaded
                        var existingBook = db.Books.AsNoTracking().FirstOrDefault(b => b.Book_id == book.Book_id);
                        if (existingBook != null)
                        {
                            book.Image = existingBook.Image;
                        }
                    }

                    db.Entry(book).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            ViewBag.admin_id_fk = new SelectList(db.Admins, "Admin_id", "Username", book.admin_id_fk);
            ViewBag.Category_id_fk = new SelectList(db.Books_Category, "Category_id", "Name", book.Category_id_fk);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // GET: Books/Category
        public ActionResult Category(string searchTerm)
        {
            var books = db.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                books = books.Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));
            }
            return View("~/Views/Home/Category.cshtml", books.ToList());
        }
    }
}
