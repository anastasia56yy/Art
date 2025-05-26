using System.Net;
using System.Web.Mvc;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.Models;

namespace ArtTicket.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IEventBL _eventBL;
        private readonly IUserBL _userBL;

        public CategoriesController()
        {
            var factory = BusinessLogicFactory.Instance;
            _eventBL = factory.GetEventBL();
            _userBL = factory.GetUserBL();
        }

        // GET: Categories
        public ActionResult Index()
        {
            var categories = _eventBL.GetCategories();
            return View(categories);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = _eventBL.GetCategoryById(id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // GET: Categories/Admin
        [Authorize]
        public ActionResult Admin()
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var categories = _eventBL.GetCategories();
            return View(categories);
        }

        // GET: Categories/Create
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

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Name,Description")] EventCategory category)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (ModelState.IsValid)
            {
                _eventBL.CreateCategory(category);
                return RedirectToAction("Admin");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
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

            var category = _eventBL.GetCategoryById(id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] EventCategory category)
        {
            // Проверяем, имеет ли пользователь права администратора
            if (!_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (ModelState.IsValid)
            {
                _eventBL.UpdateCategory(category);
                return RedirectToAction("Admin");
            }

            return View(category);
        }

        // GET: Categories/Delete/5
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

            var category = _eventBL.GetCategoryById(id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Проверяем, есть ли мероприятия, связанные с этой категорией
            var hasEvents = _eventBL.HasEventsForCategory(id.Value);
            ViewBag.HasEvents = hasEvents;

            return View(category);
        }

        // POST: Categories/Delete/5
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
            
            // Проверяем, есть ли мероприятия, связанные с этой категорией
            var hasEvents = _eventBL.HasEventsForCategory(id);
            if (hasEvents)
            {
                var category = _eventBL.GetCategoryById(id);
                ModelState.AddModelError("", "Невозможно удалить категорию, так как с ней связаны мероприятия.");
                ViewBag.HasEvents = true;
                return View("Delete", category);
            }

            _eventBL.DeleteCategory(id);
            return RedirectToAction("Admin");
        }
    }
} 