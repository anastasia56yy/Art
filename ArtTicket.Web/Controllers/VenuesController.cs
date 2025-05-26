using System.Net;
using System.Web.Mvc;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.Models;

namespace ArtTicket.Web.Controllers
{
    public class VenuesController : Controller
    {
        private readonly IEventBL _eventBL;
        private readonly IUserBL _userBL;

        public VenuesController()
        {
            var factory = BusinessLogicFactory.Instance;
            _eventBL = factory.GetEventBL();
            _userBL = factory.GetUserBL();
        }

        // GET: Venues
        public ActionResult Index()
        {
            var venues = _eventBL.GetVenues();
            return View(venues);
        }

        // GET: Venues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var venue = _eventBL.GetVenueById(id.Value);

            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // GET: Venues/Admin
        [Authorize]
        public ActionResult Admin()
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var venues = _eventBL.GetVenues();
            return View(venues);
        }

        // GET: Venues/Create
        [Authorize]
        public ActionResult Create()
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Name,Address,City,Description,Capacity,ImageUrl")] Venue venue)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (ModelState.IsValid)
            {
                _eventBL.CreateVenue(venue);
                return RedirectToAction("Admin");
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var venue = _eventBL.GetVenueById(id.Value);
            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,City,Description,Capacity,ImageUrl")] Venue venue)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (ModelState.IsValid)
            {
                _eventBL.UpdateVenue(venue);
                return RedirectToAction("Admin");
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var venue = _eventBL.GetVenueById(id.Value);
            if (venue == null)
            {
                return HttpNotFound();
            }

            // Проверяем, есть ли мероприятия, связанные с этой площадкой
            var hasEvents = _eventBL.HasEventsForVenue(id.Value);
            ViewBag.HasEvents = hasEvents;

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Проверяем, есть ли мероприятия, связанные с этой площадкой
            var hasEvents = _eventBL.HasEventsForVenue(id);
            if (hasEvents)
            {
                var venue = _eventBL.GetVenueById(id);
                ModelState.AddModelError("", "Невозможно удалить площадку, так как с ней связаны мероприятия.");
                ViewBag.HasEvents = true;
                return View("Delete", venue);
            }

            _eventBL.DeleteVenue(id);
            return RedirectToAction("Admin");
        }
    }
} 