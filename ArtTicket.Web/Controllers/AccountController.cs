using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ArtTicketDbContext _dbContext;

        public AccountController()
        {
            _dbContext = new ArtTicketDbContext();
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, bool? rememberMe = false, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Введите email и пароль");
                return View();
            }

            // Простая проверка учетных данных
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Неверный email или пароль");
                return View();
            }

            // Аутентификация прошла успешно
            FormsAuthentication.SetAuthCookie(email, rememberMe ?? false);

            // Сохраняем дополнительные данные в сессии
            Session["UserId"] = user.Id;
            Session["UserName"] = $"{user.FirstName} {user.LastName}";
            Session["UserRole"] = user.Role;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string firstName, string lastName, string email, string phoneNumber, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || 
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Все поля обязательны для заполнения");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают");
                return View();
            }

            // Проверяем, что пользователь с таким email еще не зарегистрирован
            if (_dbContext.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "Пользователь с таким email уже существует");
                return View();
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

            // Автоматически авторизуем пользователя после регистрации
            FormsAuthentication.SetAuthCookie(email, false);

            Session["UserId"] = user.Id;
            Session["UserName"] = $"{user.FirstName} {user.LastName}";
            Session["UserRole"] = user.Role;

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            
            // Очищаем сессию
            Session.Clear();
            Session.Abandon();

            // Очищаем куки аутентификации
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        [Authorize]
        public ActionResult Profile()
        {
            var email = User.Identity.Name;
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Вспомогательные методы для работы с паролями
        private string HashPassword(string password)
        {
            // Простая реализация хеширования пароля
            // В реальном проекте используйте более надежное хеширование
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Проверка соответствия пароля хешу
            var hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            return hashedPassword == passwordHash;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 