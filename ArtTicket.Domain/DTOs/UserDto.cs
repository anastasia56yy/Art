using System;
using System.Collections.Generic;

namespace ArtTicket.Domain.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Role { get; set; }
        public List<OrderDto> Orders { get; set; }
        
        public UserDto()
        {
            Orders = new List<OrderDto>();
        }
    }
} 