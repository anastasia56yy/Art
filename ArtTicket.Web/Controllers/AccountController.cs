using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ArtTicket.Application;
using ArtTicket.Application.Interfaces;
using ArtTicket.Web.Models.ViewModels;

namespace ArtTicket.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserBL _userBL;

        public AccountController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _userBL.Login(model.Email, model.Password, model.RememberMe);

            if (!result.IsAuthenticated)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            // Аутентификация прошла успешно
            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

            // Сохраняем дополнительные данные в сессии
            Session["UserId"] = result.UserId;
            Session["UserName"] = result.UserName;
            Session["UserRole"] = result.Role;

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _userBL.Register(model.FirstName, model.LastName, model.Email, model.PhoneNumber, model.Password);

            if (!result.IsAuthenticated)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            // Автоматически авторизуем пользователя после регистрации
            FormsAuthentication.SetAuthCookie(model.Email, false);

            Session["UserId"] = result.UserId;
            Session["UserName"] = result.UserName;
            Session["UserRole"] = result.Role;

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
            var userDto = _userBL.GetUserByEmail(email);
            
            if (userDto == null)
            {
                return RedirectToAction("Login");
            }

            return View(userDto);
        }
    }
} 