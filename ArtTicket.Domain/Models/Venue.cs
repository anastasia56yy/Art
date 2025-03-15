using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        
        // Навигационные свойства
        public virtual ICollection<Event> Events { get; set; }
    }
} 