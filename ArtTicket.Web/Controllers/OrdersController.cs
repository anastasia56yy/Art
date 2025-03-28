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
    public class OrdersController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;

        public OrdersController()
        {
            _dbContext = new ArtTicketDbContext();
        }

        // GET: Orders/Admin
        public ActionResult Admin()
        {
            var orders = _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Tickets)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Tickets)
                .Include("Tickets.Event")
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Только администратор или владелец заказа может его просматривать
            if (!User.IsInRole("Admin") && order.User.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(order);
        }

        // GET: Orders/Create/5 (id мероприятия)
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
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", new { eventId = eventId }) });
            }

            var orderViewModel = new OrderViewModel
            {
                Event = eventItem,
                TicketCount = 1,
                User = user
            };

            return View(orderViewModel);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventItem = _dbContext.Events.Find(model.EventId);
                if (eventItem == null)
                {
                    return HttpNotFound();
                }

                // Получаем текущего пользователя
                var userEmail = User.Identity.Name;
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Создаем новый заказ
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = eventItem.Price * model.TicketCount,
                    Status = "Pending",
                    UserId = user.Id,
                    Tickets = new List<Ticket>()
                };

                // Добавляем билеты к заказу
                for (int i = 0; i < model.TicketCount; i++)
                {
                    var ticket = new Ticket
                    {
                        Price = eventItem.Price,
                        IsAvailable = false,
                        SeatNumber = "Место " + (i + 1), // Упрощенно
                        EventId = eventItem.Id
                    };
                    order.Tickets.Add(ticket);
                }

                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();

                // Перенаправляем на страницу подтверждения заказа
                return RedirectToAction("Confirmation", new { id = order.Id });
            }

            // Если модель невалидна, перезагружаем данные
            var eventForReload = _dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == model.EventId);

            var userForReload = _dbContext.Users.FirstOrDefault(u => u.Email == User.Identity.Name);

            model.Event = eventForReload;
            model.User = userForReload;

            return View(model);
        }

        // GET: Orders/Confirmation/5
        public ActionResult Confirmation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Tickets)
                .Include("Tickets.Event")
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Только владелец заказа может его просматривать
            if (order.User.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(order);
        }

        // GET: Orders/Cancel/5
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Tickets)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Только администратор или владелец заказа может его отменить
            if (!User.IsInRole("Admin") && order.User.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Можно отменить только заказы в статусе "Pending"
            if (order.Status != "Pending")
            {
                TempData["Error"] = "Нельзя отменить заказ, который уже оплачен или отменен.";
                return RedirectToAction("Details", new { id = order.Id });
            }

            return View(order);
        }

        // POST: Orders/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmed(int id)
        {
            return ProcessCancellation(id);
        }
        
        // GET: Orders/CancelOrder/5
        [Authorize]
        public ActionResult CancelOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return ProcessCancellation(id.Value);
        }
        
        // GET: Orders/CancelConfirmed/5
        public ActionResult CancelConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return ProcessCancellation(id.Value);
        }
        
        // Общий метод для обработки отмены заказа
        private ActionResult ProcessCancellation(int id)
        {
            var order = _dbContext.Orders
                .Include(o => o.Tickets)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Только администратор или владелец заказа может его отменить
            if (!User.IsInRole("Admin") && order.User.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Меняем статус заказа на "Cancelled"
            order.Status = "Cancelled";

            // Обновляем статус всех билетов
            foreach (var ticket in order.Tickets)
            {
                ticket.IsAvailable = true;
            }

            _dbContext.SaveChanges();

            TempData["Success"] = "Заказ успешно отменен.";
            return RedirectToAction("Details", new { id = order.Id });
        }

        // GET: Orders/ChangeStatus/5
        public ActionResult ChangeStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Tickets)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending", Text = "Ожидает оплаты" },
                new SelectListItem { Value = "Paid", Text = "Оплачен" },
                new SelectListItem { Value = "Cancelled", Text = "Отменен" }
            };

            return View(order);
        }

        // POST: Orders/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(int id, string status)
        {
            var order = _dbContext.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.Status = status;
            _dbContext.SaveChanges();

            TempData["Success"] = "Статус заказа успешно изменен.";
            return RedirectToAction("Admin");
        }

        // GET: Orders/MyOrders
        public ActionResult MyOrders()
        {
            var userEmail = User.Identity.Name;
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = _dbContext.Orders
                .Include(o => o.Tickets)
                .Include("Tickets.Event")
                .Include("Tickets.Event.Venue")
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // POST: Orders/ProcessPayment/5
        [HttpPost]
        public ActionResult ProcessPayment(int id)
        {
            var order = _dbContext.Orders
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Проверяем, что заказ принадлежит текущему пользователю
            if (order.User.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Проверяем, что заказ в статусе "Pending"
            if (order.Status != "Pending")
            {
                return Json(new { success = false, message = "Заказ не может быть оплачен (не в статусе ожидания оплаты)" });
            }

            // Имитация успешной оплаты
            order.Status = "Paid";
            _dbContext.SaveChanges();

            return Json(new { success = true, message = "Оплата успешно выполнена! Ваши электронные билеты отправлены на email." });
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

    // Модель представления для создания заказа
    public class OrderViewModel
    {
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int TicketCount { get; set; }
        public User User { get; set; }
    }
} 