using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Pending, Paid, Cancelled
        public int UserId { get; set; }
        
        // Навигационные свойства
        public virtual User User { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
} 