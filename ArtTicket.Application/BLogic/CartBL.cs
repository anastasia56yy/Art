using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.BLogic
{
    public class CartBL : ICartBL
    {
        private readonly ArtTicketDbContext _dbContext;
        private readonly IUserBL _userBL;

        public CartBL(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
            _userBL = new UserBL(dbContext); // Временное решение, в идеале использовать DI
        }

        public CartResultDto GetCart(string userEmail)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Пользователь не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            var cartItems = _dbContext.CartItems
                .Include("Ticket.Event")
                .Include("Ticket.TicketType")
                .Where(ci => ci.UserId == user.Id)
                .ToList();

            var result = new CartResultDto
            {
                Success = true,
                Items = cartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    TicketId = ci.TicketId,
                    EventTitle = ci.Ticket.Event.Title,
                    TicketTypeName = ci.Ticket.TicketType?.Name,
                    Price = ci.Ticket.Price,
                    Quantity = ci.Quantity
                }).ToList(),
                TotalPrice = cartItems.Sum(ci => ci.Ticket.Price * ci.Quantity),
                ItemsCount = cartItems.Sum(ci => ci.Quantity)
            };

            return result;
        }

        public CartResultDto AddToCart(string userEmail, int ticketId, int quantity)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Пользователь не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            var ticket = _dbContext.Tickets.Find(ticketId);
            
            if (ticket == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Билет не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            // Проверяем, есть ли уже такой билет в корзине
            var existingCartItem = _dbContext.CartItems
                .FirstOrDefault(ci => ci.UserId == user.Id && ci.TicketId == ticketId);

            if (existingCartItem != null)
            {
                // Увеличиваем количество
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Добавляем новый элемент в корзину
                var cartItem = new CartItem
                {
                    UserId = user.Id,
                    TicketId = ticketId,
                    Quantity = quantity
                };

                _dbContext.CartItems.Add(cartItem);
            }

            _dbContext.SaveChanges();

            return GetCart(userEmail);
        }

        public CartResultDto RemoveFromCart(string userEmail, int cartItemId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Пользователь не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            var cartItem = _dbContext.CartItems.Find(cartItemId);
            
            if (cartItem == null || cartItem.UserId != user.Id)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Элемент корзины не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            _dbContext.CartItems.Remove(cartItem);
            _dbContext.SaveChanges();

            return GetCart(userEmail);
        }

        public CartResultDto UpdateCartItemQuantity(string userEmail, int cartItemId, int quantity)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Пользователь не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            var cartItem = _dbContext.CartItems.Find(cartItemId);
            
            if (cartItem == null || cartItem.UserId != user.Id)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Элемент корзины не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            if (quantity <= 0)
            {
                _dbContext.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            _dbContext.SaveChanges();

            return GetCart(userEmail);
        }

        public CartResultDto ClearCart(string userEmail)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (user == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorMessage = "Пользователь не найден",
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                    ItemsCount = 0
                };
            }

            var cartItems = _dbContext.CartItems.Where(ci => ci.UserId == user.Id).ToList();
            
            foreach (var item in cartItems)
            {
                _dbContext.CartItems.Remove(item);
            }

            _dbContext.SaveChanges();

            return new CartResultDto
            {
                Success = true,
                Items = new List<CartItemDto>(),
                TotalPrice = 0,
                ItemsCount = 0
            };
        }
    }
} 