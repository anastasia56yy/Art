using System.Collections.Generic;
using ArtTicket.Domain.DTOs;
using ArtTicket.Domain.Models;

namespace ArtTicket.Application.Interfaces
{
    public interface IEventBL
    {
        List<EventDto> GetAllEvents();
        EventDto GetEventById(int id);
        List<EventDto> GetEventsByCategory(int categoryId);
        void CreateEvent(EventDto eventDto);
        void UpdateEvent(EventDto eventDto);
        void DeleteEvent(int id);
        
        // Методы для получения справочников
        List<Venue> GetVenues();
        List<EventCategory> GetCategories();
        
        // Методы для работы с площадками
        Venue GetVenueById(int id);
        void CreateVenue(Venue venue);
        void UpdateVenue(Venue venue);
        void DeleteVenue(int id);
        bool HasEventsForVenue(int venueId);
        
        // Методы для работы с категориями
        EventCategory GetCategoryById(int id);
        void CreateCategory(EventCategory category);
        void UpdateCategory(EventCategory category);
        void DeleteCategory(int id);
        bool HasEventsForCategory(int categoryId);
    }
} 