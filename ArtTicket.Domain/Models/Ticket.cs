namespace ArtTicket.Domain.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string SeatNumber { get; set; }
        public int EventId { get; set; }
        public int? TicketTypeId { get; set; }
        
        // Навигационные свойства
        public virtual Event Event { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual Order Order { get; set; }
    }
} 