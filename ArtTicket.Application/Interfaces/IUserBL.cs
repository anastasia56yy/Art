using ArtTicket.Domain.DTOs;

namespace ArtTicket.Application.Interfaces
{
    public interface IUserBL
    {
        AuthResultDto Login(string email, string password, bool rememberMe);
        AuthResultDto Register(string firstName, string lastName, string email, string phoneNumber, string password);
        UserDto GetUserByEmail(string email);
        bool IsUserAdmin(string email);
        bool CanUserAccessReview(string userEmail, int reviewId);
        void UpdateUser(UserDto userDto);
    }
} 