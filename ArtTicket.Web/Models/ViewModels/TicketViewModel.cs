using System.ComponentModel.DataAnnotations;

namespace ArtTicket.Web.Models.ViewModels
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Введите цену")]
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        public int EventId { get; set; }
        
        [Display(Name = "Событие")]
        public string EventTitle { get; set; }
        
        [Display(Name = "Тип билета")]
        public int? TicketTypeId { get; set; }
        
        [Display(Name = "Тип билета")]
        public string TicketTypeName { get; set; }
    }
} 