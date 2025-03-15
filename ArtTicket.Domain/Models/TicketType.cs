using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PriceMultiplier { get; set; } // Множитель цены по сравнению с обычным билетом
        
        // Навигационные свойства
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
} 