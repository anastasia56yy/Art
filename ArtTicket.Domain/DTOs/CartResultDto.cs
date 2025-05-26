using System.Collections.Generic;

namespace ArtTicket.Domain.DTOs
{
    public class CartResultDto
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public int ItemsCount { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string EventTitle { get; set; }
        public string TicketTypeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
} 