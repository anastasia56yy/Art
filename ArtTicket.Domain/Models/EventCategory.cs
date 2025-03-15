using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Навигационные свойства
        public virtual ICollection<Event> Events { get; set; }
    }
} 