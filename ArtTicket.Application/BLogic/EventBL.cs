using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.BLogic
{
    public class EventBL : IEventBL
    {
        private readonly ArtTicketDbContext _dbContext;

        public EventBL(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<EventDto> GetAllEvents()
        {
            var events = _dbContext.Events
                .Include("Venue")
                .Include("Category")
                .OrderBy(e => e.StartDate)
                .ToList();

            return events.Select(MapToDto).ToList();
        }

        public EventDto GetEventById(int id)
        {
            var eventItem = _dbContext.Events
                .Include("Venue")
                .Include("Category")
                .Include("Tickets")
                .Include("Reviews")
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null)
            {
                return null;
            }

            return MapToDto(eventItem);
        }

        public List<EventDto> GetEventsByCategory(int categoryId)
        {
            var events = _dbContext.Events
                .Include("Venue")
                .Include("Category")
                .Where(e => e.CategoryId == categoryId)
                .OrderBy(e => e.StartDate)
                .ToList();

            return events.Select(MapToDto).ToList();
        }

        public void CreateEvent(EventDto eventDto)
        {
            var eventItem = new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                ImageUrl = eventDto.ImageUrl,
                IsFeatured = eventDto.IsFeatured,
                Price = eventDto.Price,
                VenueId = eventDto.VenueId,
                CategoryId = eventDto.CategoryId
            };

            _dbContext.Events.Add(eventItem);
            _dbContext.SaveChanges();
        }

        public void UpdateEvent(EventDto eventDto)
        {
            var eventItem = _dbContext.Events.Find(eventDto.Id);
            
            if (eventItem != null)
            {
                eventItem.Title = eventDto.Title;
                eventItem.Description = eventDto.Description;
                eventItem.StartDate = eventDto.StartDate;
                eventItem.EndDate = eventDto.EndDate;
                eventItem.ImageUrl = eventDto.ImageUrl;
                eventItem.IsFeatured = eventDto.IsFeatured;
                eventItem.Price = eventDto.Price;
                eventItem.VenueId = eventDto.VenueId;
                eventItem.CategoryId = eventDto.CategoryId;
                
                _dbContext.SaveChanges();
            }
        }

        public void DeleteEvent(int id)
        {
            var eventItem = _dbContext.Events.Find(id);
            
            if (eventItem != null)
            {
                _dbContext.Events.Remove(eventItem);
                _dbContext.SaveChanges();
            }
        }
        
        public List<Venue> GetVenues()
        {
            return _dbContext.Venues.OrderBy(v => v.Name).ToList();
        }
        
        public List<EventCategory> GetCategories()
        {
            return _dbContext.EventCategories.OrderBy(c => c.Name).ToList();
        }
        
        public Venue GetVenueById(int id)
        {
            return _dbContext.Venues
                .Include("Events")
                .FirstOrDefault(v => v.Id == id);
        }
        
        public void CreateVenue(Venue venue)
        {
            _dbContext.Venues.Add(venue);
            _dbContext.SaveChanges();
        }
        
        public void UpdateVenue(Venue venue)
        {
            _dbContext.Entry(venue).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        
        public void DeleteVenue(int id)
        {
            var venue = _dbContext.Venues.Find(id);
            
            if (venue != null)
            {
                _dbContext.Venues.Remove(venue);
                _dbContext.SaveChanges();
            }
        }
        
        public bool HasEventsForVenue(int venueId)
        {
            return _dbContext.Events.Any(e => e.VenueId == venueId);
        }
        
        public EventCategory GetCategoryById(int id)
        {
            return _dbContext.EventCategories
                .Include("Events")
                .FirstOrDefault(c => c.Id == id);
        }
        
        public void CreateCategory(EventCategory category)
        {
            _dbContext.EventCategories.Add(category);
            _dbContext.SaveChanges();
        }
        
        public void UpdateCategory(EventCategory category)
        {
            _dbContext.Entry(category).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        
        public void DeleteCategory(int id)
        {
            var category = _dbContext.EventCategories.Find(id);
            
            if (category != null)
            {
                _dbContext.EventCategories.Remove(category);
                _dbContext.SaveChanges();
            }
        }
        
        public bool HasEventsForCategory(int categoryId)
        {
            return _dbContext.Events.Any(e => e.CategoryId == categoryId);
        }

        private EventDto MapToDto(Event eventItem)
        {
            var tickets = eventItem.Tickets?.Select(t => new TicketDto
            {
                Id = t.Id,
                Price = t.Price,
                EventId = t.EventId,
                EventTitle = eventItem.Title,
                TicketTypeId = t.TicketTypeId,
                TicketTypeName = t.TicketType?.Name
            }).ToList() ?? new List<TicketDto>();

            var reviews = eventItem.Reviews?.Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate,
                EventId = r.EventId,
                EventTitle = eventItem.Title,
                UserId = r.UserId,
                UserName = $"{r.User?.FirstName} {r.User?.LastName}",
                UserEmail = r.User?.Email
            }).ToList() ?? new List<ReviewDto>();

            return new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                StartDate = eventItem.StartDate,
                EndDate = eventItem.EndDate,
                ImageUrl = eventItem.ImageUrl,
                IsFeatured = eventItem.IsFeatured,
                Price = eventItem.Price,
                VenueId = eventItem.VenueId,
                VenueName = eventItem.Venue?.Name,
                CategoryId = eventItem.CategoryId,
                CategoryName = eventItem.Category?.Name,
                Tickets = tickets,
                Reviews = reviews
            };
        }
    }
} 