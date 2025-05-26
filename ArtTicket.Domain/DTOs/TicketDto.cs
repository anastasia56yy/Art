namespace ArtTicket.Domain.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public int? TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
    }
} 