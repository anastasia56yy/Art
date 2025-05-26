using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArtTicket.Web.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        
        [Display(Name = "Общая сумма")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
        
        [Display(Name = "Количество товаров")]
        public int ItemsCount { get; set; }
    }
    
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        
        [Display(Name = "Событие")]
        public string EventTitle { get; set; }
        
        [Display(Name = "Тип билета")]
        public string TicketTypeName { get; set; }
        
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        [Display(Name = "Количество")]
        [Range(1, 10, ErrorMessage = "Количество должно быть от 1 до 10")]
        public int Quantity { get; set; }
    }
} 