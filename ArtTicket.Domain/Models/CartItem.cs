using System;

namespace ArtTicket.Domain.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        
        public virtual User User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
} 