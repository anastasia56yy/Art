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
    public class VenuesController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;

        public VenuesController()
        {
            _dbContext = new ArtTicketDbContext();
        }

        // GET: Venues
        public ActionResult Index()
        {
            var venues = _dbContext.Venues.ToList();
            return View(venues);
        }

        // GET: Venues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var venue = _dbContext.Venues
                .Include(v => v.Events)
                .FirstOrDefault(v => v.Id == id);

            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // GET: Venues/Admin
        public ActionResult Admin()
        {
            var venues = _dbContext.Venues.ToList();
            return View(venues);
        }

        // GET: Venues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Address,City,Description,Capacity,ImageUrl")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Venues.Add(venue);
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var venue = _dbContext.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,City,Description,Capacity,ImageUrl")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(venue).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var venue = _dbContext.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }

            // Проверяем, есть ли мероприятия, связанные с этой площадкой
            var hasEvents = _dbContext.Events.Any(e => e.VenueId == id);
            ViewBag.HasEvents = hasEvents;

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var venue = _dbContext.Venues.Find(id);

            // Проверяем, есть ли мероприятия, связанные с этой площадкой
            var hasEvents = _dbContext.Events.Any(e => e.VenueId == id);
            if (hasEvents)
            {
                ModelState.AddModelError("", "Невозможно удалить площадку, так как с ней связаны мероприятия.");
                ViewBag.HasEvents = true;
                return View(venue);
            }

            _dbContext.Venues.Remove(venue);
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