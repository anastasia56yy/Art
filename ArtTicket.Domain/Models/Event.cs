using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public decimal Price { get; set; }
        public int VenueId { get; set; }
        public int CategoryId { get; set; }
        
        // Навигационные свойства
        public virtual Venue Venue { get; set; }
        public virtual EventCategory Category { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
} 