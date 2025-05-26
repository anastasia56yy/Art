using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArtTicket.Web.Models.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Дата заказа")]
        public DateTime OrderDate { get; set; }
        
        [Display(Name = "Общая сумма")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
        
        [Display(Name = "Статус")]
        public string Status { get; set; }
        
        public int UserId { get; set; }
        
        [Display(Name = "Пользователь")]
        public string UserName { get; set; }
        
        public List<OrderItemViewModel> Items { get; set; }
        
        // Добавляем недостающие свойства для Create.cshtml
        public int EventId { get; set; }
        public EventViewModel Event { get; set; }
        
        [Display(Name = "Количество билетов")]
        [Range(1, 10, ErrorMessage = "Количество билетов должно быть от 1 до 10")]
        public int TicketCount { get; set; } = 1;
    }
    
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }
        public int EventId { get; set; }
        
        [Display(Name = "Событие")]
        public string EventTitle { get; set; }
        
        [Display(Name = "Тип билета")]
        public string TicketTypeName { get; set; }
        
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        
        // Добавляем свойство Event для MyOrders.cshtml
        public EventViewModel Event { get; set; }
    }
    
    public class EventGroupViewModel
    {
        public EventViewModel Event { get; set; }
        public int TicketCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 