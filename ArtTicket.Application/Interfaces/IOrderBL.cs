using System.Collections.Generic;
using ArtTicket.Domain.DTOs;

namespace ArtTicket.Application.Interfaces
{
    public interface IOrderBL
    {
        List<OrderDto> GetUserOrders(string userEmail);
        OrderDto GetOrderById(int id, string userEmail);
        OrderDto CreateOrderFromCart(string userEmail);
        OrderDto CreateOrderForEvent(string userEmail, int eventId, int ticketCount);
        void UpdateOrderStatus(int orderId, string status, string userEmail);
        bool CanUserAccessOrder(string userEmail, int orderId);
    }
} 