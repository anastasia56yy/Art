using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ArtTicket.Application.Services;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Web.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;
        private readonly ReviewService _reviewService;
        private readonly UserService _userService;

        public ReviewsController()
        {
            _dbContext = new ArtTicketDbContext();
            _reviewService = new ReviewService(_dbContext);
            _userService = new UserService(_dbContext);
        }

        // GET: Reviews/Index/5 (eventId)
        public ActionResult Index(int? eventId)
        {
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventItem = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == eventId);

            if (eventItem == null)
            {
                return HttpNotFound();
            }

            var reviews = _reviewService.GetReviewsByEventId(eventId.Value);

            ViewBag.Event = eventItem;
            return View(reviews);
        }

        // GET: Reviews/Create/5 (eventId)
        [Authorize]
        public ActionResult Create(int? eventId)
        {
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventItem = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == eventId);

            if (eventItem == null)
            {
                return HttpNotFound();
            }

            // Получаем текущего пользователя
            var userEmail = User.Identity.Name;
            var user = _userService.GetUserByEmail(userEmail);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", new { eventId = eventId }) });
            }

            // Проверяем, оставил ли уже пользователь отзыв на это мероприятие
            bool hasReview = _reviewService.UserHasReviewForEvent(user.Id, eventId.Value);

            if (hasReview)
            {
                TempData["Error"] = "Вы уже оставили отзыв на это мероприятие.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var reviewViewModel = new ReviewViewModel
            {
                EventId = eventId.Value,
                Event = eventItem,
                Rating = 5 // По умолчанию 5 звезд
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
                var user = _userService.GetUserByEmail(userEmail);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Проверяем, оставил ли уже пользователь отзыв на это мероприятие
                bool hasReview = _reviewService.UserHasReviewForEvent(user.Id, model.EventId);

                if (hasReview)
                {
                    TempData["Error"] = "Вы уже оставили отзыв на это мероприятие.";
                    return RedirectToAction("Details", "Events", new { id = model.EventId });
                }

                // Создаем новый отзыв
                var review = new Review
                {
                    Comment = model.Comment,
                    Rating = model.Rating,
                    CreatedDate = DateTime.Now,
                    EventId = model.EventId,
                    UserId = user.Id
                };

                _reviewService.CreateReview(review);

                TempData["Success"] = "Ваш отзыв успешно добавлен.";
                return RedirectToAction("Details", "Events", new { id = model.EventId });
            }

            // Если модель невалидна, перезагружаем данные
            var eventForReload = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == model.EventId);

            model.Event = eventForReload;

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

            var review = _reviewService.GetReviewById(id.Value);

            if (review == null)
            {
                return HttpNotFound();
            }

            // Проверяем права доступа пользователя
            string userEmail = User.Identity.Name;
            if (!_userService.CanUserAccessReview(userEmail, id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var reviewViewModel = new ReviewViewModel
            {
                Id = review.Id,
                EventId = review.EventId,
                Event = review.Event,
                Comment = review.Comment,
                Rating = review.Rating
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
                var review = _reviewService.GetReviewById(model.Id);

                if (review == null)
                {
                    return HttpNotFound();
                }

                // Проверяем права доступа пользователя
                string userEmail = User.Identity.Name;
                if (!_userService.CanUserAccessReview(userEmail, model.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Обновляем отзыв
                review.Comment = model.Comment;
                review.Rating = model.Rating;
                _reviewService.UpdateReview(review);

                TempData["Success"] = "Ваш отзыв успешно обновлен.";
                return RedirectToAction("MyReviews", "Reviews");
            }

            // Если модель невалидна, перезагружаем данные
            var eventForReload = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == model.EventId);

            model.Event = eventForReload;

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

            var review = _reviewService.GetReviewById(id.Value);

            if (review == null)
            {
                return HttpNotFound();
            }

            // Проверяем права доступа пользователя
            string userEmail = User.Identity.Name;
            if (!_userService.CanUserAccessReview(userEmail, id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var review = _reviewService.GetReviewById(id);
            if (review == null)
            {
                return HttpNotFound();
            }

            // Проверяем права доступа пользователя
            string userEmail = User.Identity.Name;
            if (!_userService.CanUserAccessReview(userEmail, id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            _reviewService.DeleteReview(id);

            TempData["Success"] = "Отзыв успешно удален.";
            return RedirectToAction("MyReviews", "Reviews");
        }

        // GET: Reviews/MyReviews
        [Authorize]
        public ActionResult MyReviews()
        {
            var userEmail = User.Identity.Name;
            var user = _userService.GetUserByEmail(userEmail);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var reviews = _reviewService.GetReviewsByUserId(user.Id);

            return View(reviews);
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

    // Модель представления для отзывов
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста, введите текст отзыва")]
        [StringLength(1000, ErrorMessage = "Отзыв не должен превышать 1000 символов")]
        public string Comment { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста, укажите рейтинг")]
        [Range(1, 5, ErrorMessage = "Рейтинг должен быть от 1 до 5")]
        public int Rating { get; set; }
    }
} 