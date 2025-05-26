using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Role { get; set; } // Admin, User
        
        // Навигационные свойства
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        
        public User()
        {
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            CartItems = new HashSet<CartItem>();
        }
    }
} 