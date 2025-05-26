using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ArtTicket.Web.Models.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Введите название события")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Введите описание")]
        [Display(Name = "Описание")]
        [AllowHtml]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Введите дату начала")]
        [Display(Name = "Дата начала")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "Введите дату окончания")]
        [Display(Name = "Дата окончания")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "URL изображения")]
        public string ImageUrl { get; set; }
        
        [Display(Name = "Рекомендуемое")]
        public bool IsFeatured { get; set; }
        
        [Required(ErrorMessage = "Введите цену")]
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Выберите зал")]
        [Display(Name = "Зал")]
        public int VenueId { get; set; }
        
        [Display(Name = "Зал")]
        public string VenueName { get; set; }
        
        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        
        [Display(Name = "Категория")]
        public string CategoryName { get; set; }
        
        public VenueViewModel Venue { get; set; }
        public CategoryViewModel Category { get; set; }
        
        public List<ReviewViewModel> Reviews { get; set; }
        
        public List<TicketViewModel> Tickets { get; set; }
    }
    
    public class VenueViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
} 