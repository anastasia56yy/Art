using System.Linq;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.Services
{
    public class UserService
    {
        private readonly ArtTicketDbContext _dbContext;

        public UserService(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetUserByEmail(string email)
        {
            return _dbContext.Users
                .FirstOrDefault(u => u.Email == email);
        }

        public bool IsUserAdmin(string email)
        {
            var user = GetUserByEmail(email);
            return user != null && user.Role == "Admin";
        }
        
        public bool CanUserAccessReview(string userEmail, int reviewId)
        {
            var user = GetUserByEmail(userEmail);
            if (user == null)
            {
                return false;
            }

            // Администраторы имеют полный доступ
            if (user.Role == "Admin")
            {
                return true;
            }

            // Обычные пользователи имеют доступ только к своим отзывам
            var review = _dbContext.Reviews.Find(reviewId);
            return review != null && review.UserId == user.Id;
        }
    }
} 