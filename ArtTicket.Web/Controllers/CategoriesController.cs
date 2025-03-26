using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;

        public CategoriesController()
        {
            _dbContext = new ArtTicketDbContext();
        }

        // GET: Categories
        public ActionResult Index()
        {
            var categories = _dbContext.EventCategories.ToList();
            return View(categories);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = _dbContext.EventCategories
                .Include(c => c.Events)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // GET: Categories/Admin
        public ActionResult Admin()
        {
            var categories = _dbContext.EventCategories.Include(c => c.Events).ToList();
            return View(categories);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] EventCategory category)
        {
            if (ModelState.IsValid)
            {
                _dbContext.EventCategories.Add(category);
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = _dbContext.EventCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] EventCategory category)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(category).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }

            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = _dbContext.EventCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Проверяем, есть ли мероприятия, связанные с этой категорией
            var hasEvents = _dbContext.Events.Any(e => e.CategoryId == id);
            ViewBag.HasEvents = hasEvents;

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = _dbContext.EventCategories.Find(id);

            // Проверяем, есть ли мероприятия, связанные с этой категорией
            var hasEvents = _dbContext.Events.Any(e => e.CategoryId == id);
            if (hasEvents)
            {
                ModelState.AddModelError("", "Невозможно удалить категорию, так как с ней связаны мероприятия.");
                ViewBag.HasEvents = true;
                return View(category);
            }

            _dbContext.EventCategories.Remove(category);
            _dbContext.SaveChanges();
            return RedirectToAction("Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 