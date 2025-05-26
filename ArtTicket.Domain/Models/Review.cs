using System;

namespace ArtTicket.Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedDate { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        
        // Навигационные свойства
        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
    }
} 