using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;

namespace ArtTicket.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUserBL _userBL;
        private readonly IEventBL _eventBL;

        public AdminController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
            _eventBL = factory.GetEventBL();
        }

        // GET: Admin - Dashboard
        public ActionResult Index()
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            // Получаем статистические данные
            var events = _eventBL.GetAllEvents();
            var venues = _eventBL.GetVenues();
            var categories = _eventBL.GetCategories();

            var dashboardStats = new Dictionary<string, int>
            {
                { "EventsCount", events.Count },
                { "VenuesCount", venues.Count },
                { "CategoriesCount", categories.Count },
                { "UpcomingEventsCount", events.Count(e => e.StartDate > DateTime.Now) },
                { "OngoingEventsCount", events.Count(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now) },
                { "PastEventsCount", events.Count(e => e.EndDate < DateTime.Now) }
            };

            // Получаем недавние события
            var recentEvents = events
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
                // Dispose any other resources if needed
            }
            base.Dispose(disposing);
        }
    }
} 