using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Web.Models.ViewModels;

namespace ArtTicket.Web.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewBL _reviewBL;
        private readonly IUserBL _userBL;
        private readonly IEventBL _eventBL;

        public ReviewsController()
        {
            var factory = BusinessLogicFactory.Instance;
            _reviewBL = factory.GetReviewBL();
            _userBL = factory.GetUserBL();
            _eventBL = factory.GetEventBL();
        }

        // GET: Reviews/Index/5 (eventId)
        public ActionResult Index(int? eventId)
        {
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventDto = _eventBL.GetEventById(eventId.Value);

            if (eventDto == null)
            {
                return HttpNotFound();
            }

            var reviews = _reviewBL.GetReviewsByEventId(eventId.Value);
            var reviewViewModels = reviews.Select(r => new ReviewViewModel
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
            }).ToList();

            ViewBag.Event = new EventViewModel
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                VenueName = eventDto.VenueName,
                CategoryName = eventDto.CategoryName
            };
            
            return View(reviewViewModels);
        }

        // GET: Reviews/Create/5 (eventId)
        [Authorize]
        public ActionResult Create(int? eventId)
        {
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventDto = _eventBL.GetEventById(eventId.Value);

            if (eventDto == null)
            {
                return HttpNotFound();
            }

            // Получаем текущего пользователя
            var userEmail = User.Identity.Name;
            var userDto = _userBL.GetUserByEmail(userEmail);

            if (userDto == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", new { eventId = eventId }) });
            }

            // Проверяем, оставил ли уже пользователь отзыв на это мероприятие
            bool hasReview = _reviewBL.GetReviewsByEventId(eventId.Value)
                .Any(r => r.UserId == userDto.Id);

            if (hasReview)
            {
                TempData["Error"] = "Вы уже оставили отзыв на это мероприятие.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var reviewViewModel = new ReviewViewModel
            {
                EventId = eventId.Value,
                EventTitle = eventDto.Title,
                Rating = 5, // По умолчанию 5 звезд
                Event = MapEventToViewModel(eventDto)
            };

            return View(reviewViewModel);
        }

        // POST: Reviews/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Получаем текущего пользователя
                var userEmail = User.Identity.Name;
                var userDto = _userBL.GetUserByEmail(userEmail);

                if (userDto == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Проверяем, оставил ли уже пользователь отзыв на это мероприятие
                bool hasReview = _reviewBL.GetReviewsByEventId(model.EventId)
                    .Any(r => r.UserId == userDto.Id);

                if (hasReview)
                {
                    TempData["Error"] = "Вы уже оставили отзыв на это мероприятие.";
                    return RedirectToAction("Details", "Events", new { id = model.EventId });
                }

                // Создаем новый отзыв
                var reviewDto = new ReviewDto
                {
                    Text = model.Text,
                    Rating = model.Rating,
                    CreatedDate = DateTime.Now,
                    EventId = model.EventId,
                    UserId = userDto.Id
                };

                _reviewBL.CreateReview(reviewDto);

                TempData["Success"] = "Ваш отзыв успешно добавлен.";
                return RedirectToAction("Details", "Events", new { id = model.EventId });
            }

            // Если модель невалидна, перезагружаем данные
            var eventDto = _eventBL.GetEventById(model.EventId);
            model.EventTitle = eventDto?.Title;
            model.Event = eventDto != null ? MapEventToViewModel(eventDto) : null;

            return View(model);
        }

        // GET: Reviews/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reviewDto = _reviewBL.GetReviewById(id.Value);

            if (reviewDto == null)
            {
                return HttpNotFound();
            }

            // Проверяем права доступа пользователя
            string userEmail = User.Identity.Name;
            if (!_userBL.CanUserAccessReview(userEmail, id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var eventDto = _eventBL.GetEventById(reviewDto.EventId);

            var reviewViewModel = new ReviewViewModel
            {
                Id = reviewDto.Id,
                EventId = reviewDto.EventId,
                EventTitle = reviewDto.EventTitle,
                Text = reviewDto.Text,
                Rating = reviewDto.Rating,
                CreatedDate = reviewDto.CreatedDate,
                UserId = reviewDto.UserId,
                UserName = reviewDto.UserName,
                Event = eventDto != null ? MapEventToViewModel(eventDto) : null
            };

            return View(reviewViewModel);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reviewDto = _reviewBL.GetReviewById(model.Id);

                if (reviewDto == null)
                {
                    return HttpNotFound();
                }

                // Проверяем права доступа пользователя
                string userEmail = User.Identity.Name;
                if (!_userBL.CanUserAccessReview(userEmail, model.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Обновляем отзыв
                reviewDto.Text = model.Text;
                reviewDto.Rating = model.Rating;
                _reviewBL.UpdateReview(reviewDto);

                TempData["Success"] = "Ваш отзыв успешно обновлен.";
                return RedirectToAction("MyReviews");
            }

            // Если модель невалидна, перезагружаем данные
            var eventDto = _eventBL.GetEventById(model.EventId);
            model.EventTitle = eventDto?.Title;
            model.Event = eventDto != null ? MapEventToViewModel(eventDto) : null;

            return View(model);
        }

        // GET: Reviews/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reviewDto = _reviewBL.GetReviewById(id.Value);

            if (reviewDto == null)
            {
                return HttpNotFound();
            }

            // Проверяем права доступа пользователя
            string userEmail = User.Identity.Name;
            if (!_userBL.CanUserAccessReview(userEmail, id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var eventDto = _eventBL.GetEventById(reviewDto.EventId);

            var reviewViewModel = new ReviewViewModel
            {
                Id = reviewDto.Id,
                EventId = reviewDto.EventId,
                EventTitle = reviewDto.EventTitle,
                Text = reviewDto.Text,
                Rating = reviewDto.Rating,
                CreatedDate = reviewDto.CreatedDate,
                UserId = reviewDto.UserId,
                UserName = reviewDto.UserName,
                Event = eventDto != null ? MapEventToViewModel(eventDto) : null
            };

            return View(reviewViewModel);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Проверяем права доступа пользователя
            string userEmail = User.Identity.Name;
            if (!_userBL.CanUserAccessReview(userEmail, id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            _reviewBL.DeleteReview(id);

            TempData["Success"] = "Отзыв успешно удален.";
            return RedirectToAction("MyReviews");
        }

        // GET: Reviews/MyReviews
        [Authorize]
        public ActionResult MyReviews()
        {
            var userEmail = User.Identity.Name;
            var userDto = _userBL.GetUserByEmail(userEmail);

            if (userDto == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Получаем все отзывы пользователя
            var allReviews = _reviewBL.GetReviewsByEventId(0); // Предполагаем, что это вернет все отзывы
            var userReviews = allReviews.Where(r => r.UserId == userDto.Id).ToList();

            var reviewViewModels = userReviews.Select(r => new ReviewViewModel
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
            }).ToList();

            return View(reviewViewModels);
        }

        private EventViewModel MapEventToViewModel(EventDto eventDto)
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
                Price = eventDto.Price,
                VenueId = eventDto.VenueId,
                CategoryId = eventDto.CategoryId,
                Venue = eventDto.Venue != null ? new VenueViewModel
                {
                    Id = eventDto.Venue.Id,
                    Name = eventDto.Venue.Name,
                    Address = eventDto.Venue.Address
                } : null,
                Category = eventDto.Category != null ? new CategoryViewModel
                {
                    Id = eventDto.Category.Id,
                    Name = eventDto.Category.Name
                } : null
            };
        }
    }
} 