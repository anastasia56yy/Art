using System.Linq;
using System.Net;
using System.Web.Mvc;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Web.Models.ViewModels;

namespace ArtTicket.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderBL _orderBL;
        private readonly IUserBL _userBL;
        private readonly IEventBL _eventBL;
        private readonly ICartBL _cartBL;

        public OrdersController()
        {
            var factory = BusinessLogicFactory.Instance;
            _orderBL = factory.GetOrderBL();
            _userBL = factory.GetUserBL();
            _eventBL = factory.GetEventBL();
            _cartBL = factory.GetCartBL();
        }

        // GET: Orders/Admin
        [Authorize]
        public ActionResult Admin()
        {
            var userEmail = User.Identity.Name;
            if (!_userBL.IsUserAdmin(userEmail))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var orders = _orderBL.GetUserOrders(null); // Предполагаем, что null вернет все заказы
            var orderViewModels = orders.Select(MapToViewModel).ToList();

            return View(orderViewModels);
        }

        // GET: Orders/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userEmail = User.Identity.Name;
            var orderDto = _orderBL.GetOrderById(id.Value, userEmail);

            if (orderDto == null)
            {
                return HttpNotFound();
            }

            // Доступ к заказу проверяется внутри GetOrderById

            var orderViewModel = MapToViewModel(orderDto);
            return View(orderViewModel);
        }

        // GET: Orders/Create
        [Authorize]
        public ActionResult Create(int? eventId)
        {
            if (eventId == null)
            {
                // Если eventId не указан, работаем с корзиной
                var userEmail = User.Identity.Name;
                var cartResult = _cartBL.GetCart(userEmail);

                if (!cartResult.Success || cartResult.Items.Count == 0)
                {
                    TempData["Error"] = "Ваша корзина пуста.";
                    return RedirectToAction("Index", "Cart");
                }

                return View();
            }

            // Если eventId указан, создаем заказ для конкретного события
            var eventDto = _eventBL.GetEventById(eventId.Value);
            if (eventDto == null)
            {
                return HttpNotFound();
            }

            var orderViewModel = new OrderViewModel
            {
                EventId = eventId.Value,
                Event = MapEventToViewModel(eventDto),
                TicketCount = 1
            };

            return View(orderViewModel);
        }

        // POST: Orders/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel model)
        {
            if (model.EventId > 0)
            {
                // Создание заказа для конкретного события
                if (!ModelState.IsValid)
                {
                    var eventDto = _eventBL.GetEventById(model.EventId);
                    model.Event = MapEventToViewModel(eventDto);
                    return View(model);
                }

                var userEmail = User.Identity.Name;
                var orderDto = _orderBL.CreateOrderForEvent(userEmail, model.EventId, model.TicketCount);

                if (orderDto == null)
                {
                    TempData["Error"] = "Не удалось создать заказ.";
                    return View(model);
                }

                // Перенаправляем на страницу подтверждения заказа
                return RedirectToAction("Confirmation", new { id = orderDto.Id });
            }
            else
            {
                // Создание заказа из корзины
                var userEmail = User.Identity.Name;
                var orderDto = _orderBL.CreateOrderFromCart(userEmail);

                if (orderDto == null)
                {
                    TempData["Error"] = "Не удалось создать заказ. Возможно, ваша корзина пуста.";
                    return RedirectToAction("Index", "Cart");
                }

                // Перенаправляем на страницу подтверждения заказа
                return RedirectToAction("Confirmation", new { id = orderDto.Id });
            }
        }

        // GET: Orders/Confirmation/5
        [Authorize]
        public ActionResult Confirmation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userEmail = User.Identity.Name;
            var orderDto = _orderBL.GetOrderById(id.Value, userEmail);

            if (orderDto == null)
            {
                return HttpNotFound();
            }

            // Доступ к заказу проверяется внутри GetOrderById

            var orderViewModel = MapToViewModel(orderDto);
            return View(orderViewModel);
        }

        // GET: Orders/Cancel/5
        [Authorize]
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userEmail = User.Identity.Name;
            var orderDto = _orderBL.GetOrderById(id.Value, userEmail);

            if (orderDto == null)
            {
                return HttpNotFound();
            }

            // Доступ к заказу проверяется внутри GetOrderById

            var orderViewModel = MapToViewModel(orderDto);
            return View(orderViewModel);
        }

        // POST: Orders/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmed(int id)
        {
            var userEmail = User.Identity.Name;
            _orderBL.UpdateOrderStatus(id, "Отменен", userEmail);

            TempData["Success"] = "Заказ успешно отменен.";
            return RedirectToAction("MyOrders");
        }

        // GET: Orders/ChangeStatus/5
        [Authorize]
        public ActionResult ChangeStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userEmail = User.Identity.Name;
            if (!_userBL.IsUserAdmin(userEmail))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var orderDto = _orderBL.GetOrderById(id.Value, userEmail);

            if (orderDto == null)
            {
                return HttpNotFound();
            }

            var statuses = new[] { "Создан", "Оплачен", "Обработан", "Доставлен", "Отменен" };
            ViewBag.Statuses = new SelectList(statuses);

            var orderViewModel = MapToViewModel(orderDto);
            return View(orderViewModel);
        }

        // POST: Orders/ChangeStatus/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(int id, string status)
        {
            var userEmail = User.Identity.Name;
            if (!_userBL.IsUserAdmin(userEmail))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            _orderBL.UpdateOrderStatus(id, status, userEmail);

            TempData["Success"] = "Статус заказа успешно изменен.";
            return RedirectToAction("Admin");
        }

        // GET: Orders/MyOrders
        [Authorize]
        public ActionResult MyOrders()
        {
            var userEmail = User.Identity.Name;
            var orders = _orderBL.GetUserOrders(userEmail);
            var orderViewModels = orders.Select(MapToViewModel).ToList();

            return View(orderViewModels);
        }

        // POST: Orders/ProcessPayment/5
        [HttpPost]
        [Authorize]
        public ActionResult ProcessPayment(int id)
        {
            var userEmail = User.Identity.Name;
            var orderDto = _orderBL.GetOrderById(id, userEmail);

            if (orderDto == null)
            {
                return HttpNotFound();
            }

            // Здесь должна быть логика обработки платежа
            // В учебном примере просто меняем статус заказа
            _orderBL.UpdateOrderStatus(id, "Оплачен", userEmail);

            TempData["Success"] = "Заказ успешно оплачен.";
            return RedirectToAction("MyOrders");
        }

        private OrderViewModel MapToViewModel(OrderDto orderDto)
        {
            if (orderDto == null)
                return null;

            return new OrderViewModel
            {
                Id = orderDto.Id,
                OrderDate = orderDto.OrderDate,
                TotalPrice = orderDto.TotalPrice,
                Status = orderDto.Status,
                UserId = orderDto.UserId,
                UserName = orderDto.UserName,
                Items = orderDto.Items?.Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    OrderId = i.OrderId,
                    TicketId = i.TicketId,
                    EventId = i.EventId,
                    EventTitle = i.EventTitle,
                    TicketTypeName = i.TicketTypeName,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Event = i.Event != null ? MapEventToViewModel(i.Event) : null
                }).ToList()
            };
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