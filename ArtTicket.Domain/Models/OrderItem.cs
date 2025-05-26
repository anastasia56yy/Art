using System;

namespace ArtTicket.Domain.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        
        public virtual Order Order { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
} 