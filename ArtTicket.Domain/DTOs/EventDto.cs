using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.DTOs
{
    public class EventDto
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
        public string VenueName { get; set; }
        
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        
        public List<TicketDto> Tickets { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        
        public VenueDto Venue { get; set; }
        public CategoryDto Category { get; set; }
    }
    
    public class VenueDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
} 