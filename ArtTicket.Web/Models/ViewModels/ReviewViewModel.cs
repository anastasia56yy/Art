using System;
using System.ComponentModel.DataAnnotations;

namespace ArtTicket.Web.Models.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Введите текст отзыва")]
        [Display(Name = "Текст отзыва")]
        public string Text { get; set; }
        
        [Required(ErrorMessage = "Выберите оценку")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        [Display(Name = "Оценка")]
        public int Rating { get; set; }
        
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }
        
        public int EventId { get; set; }
        
        [Display(Name = "Событие")]
        public string EventTitle { get; set; }
        
        public int UserId { get; set; }
        
        [Display(Name = "Пользователь")]
        public string UserName { get; set; }
        
        public string UserEmail { get; set; }
        
        public EventViewModel Event { get; set; }
    }
} 