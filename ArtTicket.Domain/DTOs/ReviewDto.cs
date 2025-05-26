using System;

namespace ArtTicket.Domain.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
} 