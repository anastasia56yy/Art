using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.BLogic
{
    public class UserBL : IUserBL
    {
        private readonly ArtTicketDbContext _dbContext;

        public UserBL(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AuthResultDto Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new AuthResultDto 
                { 
                    IsAuthenticated = false, 
                    ErrorMessage = "Введите email и пароль" 
                };
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return new AuthResultDto 
                { 
                    IsAuthenticated = false, 
                    ErrorMessage = "Неверный email или пароль" 
                };
            }

            // Успешная аутентификация
            return new AuthResultDto
            {
                IsAuthenticated = true,
                UserId = user.Id.ToString(),
                UserName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role,
                ExpirationDate = rememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(30)
            };
        }

        public AuthResultDto Register(string firstName, string lastName, string email, string phoneNumber, string password)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || 
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new AuthResultDto 
                { 
                    IsAuthenticated = false, 
                    ErrorMessage = "Все поля обязательны для заполнения" 
                };
            }

            // Проверяем, что пользователь с таким email еще не зарегистрирован
            if (_dbContext.Users.Any(u => u.Email == email))
            {
                return new AuthResultDto 
                { 
                    IsAuthenticated = false, 
                    ErrorMessage = "Пользователь с таким email уже существует" 
                };
            }

            // Создаем нового пользователя
            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                RegistrationDate = DateTime.Now,
                Role = "User", // По умолчанию обычный пользователь
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Возвращаем результат успешной регистрации
            return new AuthResultDto
            {
                IsAuthenticated = true,
                UserId = user.Id.ToString(),
                UserName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role,
                ExpirationDate = DateTime.Now.AddMinutes(30)
            };
        }

        public UserDto GetUserByEmail(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                RegistrationDate = user.RegistrationDate,
                Role = user.Role
            };
        }

        public bool IsUserAdmin(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            return user != null && user.Role == "Admin";
        }

        public bool CanUserAccessReview(string userEmail, int reviewId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
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

        public void UpdateUser(UserDto userDto)
        {
            var user = _dbContext.Users.Find(userDto.Id);
            
            if (user != null)
            {
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.PhoneNumber = userDto.PhoneNumber;
                
                _dbContext.SaveChanges();
            }
        }

        // Вспомогательные методы для работы с паролями
        private string HashPassword(string password)
        {
            // Простая реализация хеширования пароля
            // В реальном проекте используйте более надежное хеширование с солью
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Проверка соответствия пароля хешу
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
        }
    }
} 