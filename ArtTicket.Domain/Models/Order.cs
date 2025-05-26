using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Создан, Оплачен, Обработан, Доставлен, Отменен
        public int UserId { get; set; }
        
        // Навигационные свойства
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }
        
        public Order()
        {
            Items = new HashSet<OrderItem>();
        }
    }
} 