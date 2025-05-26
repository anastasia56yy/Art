using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Web.Mvc;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Web.Models.ViewModels;

namespace ArtTicket.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventBL _eventBL;
        private readonly IUserBL _userBL;

        public EventsController()
        {
            var factory = BusinessLogicFactory.Instance;
            _eventBL = factory.GetEventBL();
            _userBL = factory.GetUserBL();
        }

        // GET: Events
        public ActionResult Index()
        {
            var eventDtos = _eventBL.GetAllEvents();

            if (eventDtos.Count == 0)
            {
                ViewBag.MaintenanceMode = true;
                return View();
            }

            var viewModels = eventDtos.Select(MapToViewModel).ToList();
            return View(viewModels);
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventDto = _eventBL.GetEventById(id.Value);

            if (eventDto == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(eventDto);
            return View(viewModel);
        }

        // GET: Events/Admin
        public ActionResult Admin()
        {
            // Проверка прав администратора
            if (User.Identity.IsAuthenticated && _userBL.IsUserAdmin(User.Identity.Name))
            {
                var eventDtos = _eventBL.GetAllEvents();
                var viewModels = eventDtos.Select(MapToViewModel).ToList();
                return View(viewModels);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            // Проверка прав администратора
            if (User.Identity.IsAuthenticated && _userBL.IsUserAdmin(User.Identity.Name))
            {
                // Получаем список категорий и залов через бизнес-логику
                ViewBag.VenueId = new SelectList(_eventBL.GetVenues(), "Id", "Name");
                ViewBag.CategoryId = new SelectList(_eventBL.GetCategories(), "Id", "Name");
                return View(new EventViewModel());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventViewModel model)
        {
            // Проверка прав администратора
            if (!User.Identity.IsAuthenticated || !_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var eventDto = MapToDto(model);
                _eventBL.CreateEvent(eventDto);
                return RedirectToAction("Admin");
            }

            ViewBag.VenueId = new SelectList(_eventBL.GetVenues(), "Id", "Name", model.VenueId);
            ViewBag.CategoryId = new SelectList(_eventBL.GetCategories(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            // Проверка прав администратора
            if (!User.Identity.IsAuthenticated || !_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventDto = _eventBL.GetEventById(id.Value);
            
            if (eventDto == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(eventDto);
            ViewBag.VenueId = new SelectList(_eventBL.GetVenues(), "Id", "Name", viewModel.VenueId);
            ViewBag.CategoryId = new SelectList(_eventBL.GetCategories(), "Id", "Name", viewModel.CategoryId);
            return View(viewModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventViewModel model)
        {
            // Проверка прав администратора
            if (!User.Identity.IsAuthenticated || !_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var eventDto = MapToDto(model);
                _eventBL.UpdateEvent(eventDto);
                return RedirectToAction("Admin");
            }

            ViewBag.VenueId = new SelectList(_eventBL.GetVenues(), "Id", "Name", model.VenueId);
            ViewBag.CategoryId = new SelectList(_eventBL.GetCategories(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            // Проверка прав администратора
            if (!User.Identity.IsAuthenticated || !_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventDto = _eventBL.GetEventById(id.Value);
            
            if (eventDto == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(eventDto);
            return View(viewModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Проверка прав администратора
            if (!User.Identity.IsAuthenticated || !_userBL.IsUserAdmin(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            _eventBL.DeleteEvent(id);
            return RedirectToAction("Admin");
        }

        #region Helper methods

        private EventViewModel MapToViewModel(EventDto eventDto)
        {
            if (eventDto == null)
                return null;

            return new EventViewModel
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                ImageUrl = eventDto.ImageUrl,
                IsFeatured = eventDto.IsFeatured,
                Price = eventDto.Price,
                VenueId = eventDto.VenueId,
                VenueName = eventDto.VenueName,
                CategoryId = eventDto.CategoryId,
                CategoryName = eventDto.CategoryName,
                Venue = new VenueViewModel 
                { 
                    Id = eventDto.VenueId, 
                    Name = eventDto.VenueName 
                },
                Category = new CategoryViewModel 
                {
                    Id = eventDto.CategoryId,
                    Name = eventDto.CategoryName
                },
                Tickets = eventDto.Tickets?.Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    Price = t.Price,
                    EventId = t.EventId,
                    EventTitle = t.EventTitle,
                    TicketTypeId = t.TicketTypeId,
                    TicketTypeName = t.TicketTypeName
                }).ToList() ?? new List<TicketViewModel>(),
                Reviews = eventDto.Reviews?.Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Text = r.Text,
                    Rating = r.Rating,
                    CreatedDate = r.CreatedDate,
                    EventId = r.EventId,
                    EventTitle = r.EventTitle,
                    UserId = r.UserId,
                    UserName = r.UserName,
                    UserEmail = r.UserEmail
                }).ToList() ?? new List<ReviewViewModel>()
            };
        }

        private EventDto MapToDto(EventViewModel viewModel)
        {
            if (viewModel == null)
                return null;

            return new EventDto
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                Description = viewModel.Description,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                ImageUrl = viewModel.ImageUrl,
                IsFeatured = viewModel.IsFeatured,
                Price = viewModel.Price,
                VenueId = viewModel.VenueId,
                CategoryId = viewModel.CategoryId
            };
        }

        #endregion
    }
} 