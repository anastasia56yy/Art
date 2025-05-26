using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.BLogic
{
    public class OrderBL : IOrderBL
    {
        private readonly ArtTicketDbContext _dbContext;
        private readonly IUserBL _userBL;

        public OrderBL(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
            _userBL = new UserBL(dbContext); // Временное решение, в идеале использовать DI
        }

        public List<OrderDto> GetUserOrders(string userEmail)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return new List<OrderDto>();
            }

            var orders = _dbContext.Orders
                .Include("User")
                .Include("Items.Ticket.Event")
                .Include("Items.Ticket.TicketType")
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return orders.Select(MapToDto).ToList();
        }

        public OrderDto GetOrderById(int id, string userEmail)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return null;
            }

            var order = _dbContext.Orders
                .Include("User")
                .Include("Items.Ticket.Event")
                .Include("Items.Ticket.TicketType")
                .FirstOrDefault(o => o.Id == id && (o.UserId == user.Id || user.Role == "Admin"));

            if (order == null)
            {
                return null;
            }

            return MapToDto(order);
        }

        public OrderDto CreateOrderFromCart(string userEmail)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return null;
            }

            var cartItems = _dbContext.CartItems
                .Include("Ticket.Event")
                .Include("Ticket.TicketType")
                .Where(ci => ci.UserId == user.Id)
                .ToList();

            if (!cartItems.Any())
            {
                return null;
            }

            // Создаем новый заказ
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                Status = "Создан",
                TotalPrice = cartItems.Sum(ci => ci.Ticket.Price * ci.Quantity)
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            // Добавляем элементы заказа
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    TicketId = cartItem.TicketId,
                    Price = cartItem.Ticket.Price,
                    Quantity = cartItem.Quantity
                };

                _dbContext.OrderItems.Add(orderItem);
                
                // Удаляем элемент из корзины
                _dbContext.CartItems.Remove(cartItem);
            }

            _dbContext.SaveChanges();

            return GetOrderById(order.Id, userEmail);
        }

        public OrderDto CreateOrderForEvent(string userEmail, int eventId, int ticketCount)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return null;
            }

            var eventEntity = _dbContext.Events
                .Include("Venue")
                .Include("Category")
                .FirstOrDefault(e => e.Id == eventId);

            if (eventEntity == null)
            {
                return null;
            }

            // Создаем новый заказ
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = eventEntity.Price * ticketCount
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            // Создаем элемент заказа
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                TicketId = 1, // Временное решение, нужно создать билет
                Price = eventEntity.Price,
                Quantity = ticketCount
            };

            _dbContext.OrderItems.Add(orderItem);
            _dbContext.SaveChanges();

            return GetOrderById(order.Id, userEmail);
        }

        public void UpdateOrderStatus(int orderId, string status, string userEmail)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null || user.Role != "Admin")
            {
                return;
            }

            var order = _dbContext.Orders.Find(orderId);
            
            if (order != null)
            {
                order.Status = status;
                _dbContext.SaveChanges();
            }
        }

        public bool CanUserAccessOrder(string userEmail, int orderId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return false;
            }

            // Администраторы имеют доступ ко всем заказам
            if (user.Role == "Admin")
            {
                return true;
            }

            // Обычные пользователи имеют доступ только к своим заказам
            var order = _dbContext.Orders.Find(orderId);
            return order != null && order.UserId == user.Id;
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                UserId = order.UserId,
                UserName = $"{order.User?.FirstName} {order.User?.LastName}",
                Items = order.Items?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    TicketId = oi.TicketId,
                    EventId = oi.Ticket?.Event?.Id ?? 0,
                    EventTitle = oi.Ticket?.Event?.Title ?? "Неизвестное событие",
                    TicketTypeName = oi.Ticket?.TicketType?.Name ?? "Стандартный",
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Event = oi.Ticket?.Event != null ? new EventDto
                    {
                        Id = oi.Ticket.Event.Id,
                        Title = oi.Ticket.Event.Title,
                        Description = oi.Ticket.Event.Description,
                        StartDate = oi.Ticket.Event.StartDate,
                        EndDate = oi.Ticket.Event.EndDate,
                        ImageUrl = oi.Ticket.Event.ImageUrl,
                        Price = oi.Ticket.Event.Price,
                        VenueId = oi.Ticket.Event.VenueId,
                        CategoryId = oi.Ticket.Event.CategoryId,
                        Venue = oi.Ticket.Event.Venue != null ? new VenueDto
                        {
                            Id = oi.Ticket.Event.Venue.Id,
                            Name = oi.Ticket.Event.Venue.Name,
                            Address = oi.Ticket.Event.Venue.Address
                        } : null,
                        Category = oi.Ticket.Event.Category != null ? new CategoryDto
                        {
                            Id = oi.Ticket.Event.Category.Id,
                            Name = oi.Ticket.Event.Category.Name
                        } : null
                    } : null
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
} 