using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;

        public AdminController()
        {
            _dbContext = new ArtTicketDbContext();
        }

        // GET: Admin - Dashboard
        public ActionResult Index()
        {
            var dashboardStats = new Dictionary<string, int>
            {
                { "EventsCount", _dbContext.Events.Count() },
                { "VenuesCount", _dbContext.Venues.Count() },
                { "CategoriesCount", _dbContext.EventCategories.Count() },
                { "UpcomingEventsCount", _dbContext.Events.Count(e => e.StartDate > DateTime.Now) },
                { "OngoingEventsCount", _dbContext.Events.Count(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now) },
                { "PastEventsCount", _dbContext.Events.Count(e => e.EndDate < DateTime.Now) }
            };

            // Get recent events
            var recentEvents = _dbContext.Events
                .OrderByDescending(e => e.StartDate)
                .Take(5)
                .ToList();

            ViewBag.DashboardStats = dashboardStats;
            ViewBag.RecentEvents = recentEvents;

            return View();
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