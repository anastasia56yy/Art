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
    public class EventsController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;

        public EventsController()
        {
            _dbContext = new ArtTicketDbContext();
        }

        // GET: Events
        public ActionResult Index()
        {
            var events = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .OrderBy(e => e.StartDate)
                .ToList();

            if (events.Count == 0)
            {
                ViewBag.MaintenanceMode = true;
                return View();
            }

            return View(events);
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventItem = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Tickets)
                .Include(e => e.Reviews)
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null)
            {
                return HttpNotFound();
            }

            return View(eventItem);
        }

        // GET: Events/Admin
        public ActionResult Admin()
        {
            var events = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .OrderBy(e => e.StartDate)
                .ToList();

            return View(events);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.VenueId = new SelectList(_dbContext.Venues, "Id", "Name");
            ViewBag.CategoryId = new SelectList(_dbContext.EventCategories, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,StartDate,EndDate,ImageUrl,IsFeatured,Price,VenueId,CategoryId")] Event eventItem)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Events.Add(eventItem);
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }

            ViewBag.VenueId = new SelectList(_dbContext.Venues, "Id", "Name", eventItem.VenueId);
            ViewBag.CategoryId = new SelectList(_dbContext.EventCategories, "Id", "Name", eventItem.CategoryId);
            return View(eventItem);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventItem = _dbContext.Events.Find(id);
            if (eventItem == null)
            {
                return HttpNotFound();
            }

            ViewBag.VenueId = new SelectList(_dbContext.Venues, "Id", "Name", eventItem.VenueId);
            ViewBag.CategoryId = new SelectList(_dbContext.EventCategories, "Id", "Name", eventItem.CategoryId);
            return View(eventItem);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,StartDate,EndDate,ImageUrl,IsFeatured,Price,VenueId,CategoryId")] Event eventItem)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(eventItem).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }

            ViewBag.VenueId = new SelectList(_dbContext.Venues, "Id", "Name", eventItem.VenueId);
            ViewBag.CategoryId = new SelectList(_dbContext.EventCategories, "Id", "Name", eventItem.CategoryId);
            return View(eventItem);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventItem = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null)
            {
                return HttpNotFound();
            }

            return View(eventItem);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var eventItem = _dbContext.Events.Find(id);
            _dbContext.Events.Remove(eventItem);
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