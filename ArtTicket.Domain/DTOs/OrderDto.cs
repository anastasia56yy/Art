using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        
        public int UserId { get; set; }
        public string UserName { get; set; }
        
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public string TicketTypeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public EventDto Event { get; set; }
    }
} 