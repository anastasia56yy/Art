using System;

namespace ArtTicket.Domain.DTOs
{
    public class AuthResultDto
    {
        public bool IsAuthenticated { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
} 