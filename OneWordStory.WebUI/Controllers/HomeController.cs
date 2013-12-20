using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using OneWordStory.WebUI.Attributes;
using Extensions;
using System.Web.Security;
using OneWordStory.WebUI.Controllers.Adapters;
using OneWordStory.WebUI.Models;


namespace OneWordStory.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private IUserRepository _repository;
        private IFormsAuthentication _authWrapper;

        public HomeController(IUserRepository repository, IFormsAuthentication authWrapper)
        {
            _repository = repository;
            _authWrapper = authWrapper;
        }

        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Story");
            else
                return View();
        }

        public ActionResult Logout()
        {

            var cookie = Request.Cookies[".ASPXAUTH"];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
                ViewBag.HideLogin = true;
                
            }

            return View("Index");

        }

        [HttpPost]
        public ActionResult Login(Login login)
        {

            if (!ModelState.IsValid) return View("Index");

            GetUserResult result = _repository.GetUserByEmail(login.Email);


            bool newUser = result.UserCode == UserErrorCode.EmailNotFoundInRepository;

            if (newUser)
            {
                result.UserCode = _repository.SaveUser(new User() { Email = login.Email, Password = login.Password });
            }

            if (LoginSuccessful(login, result) || newUser)
            {
                _authWrapper.SetAuthCookie(result.User.Id, true);
                return RedirectToAction("Index", "Story");

            }
            else
            {
                ModelState.AddModelError("login", "Login failed.");
                return View("Index");
            }



        }

        private static bool LoginSuccessful(Login login, GetUserResult result)
        {
            return result.UserCode == UserErrorCode.Success && result.User.Password == login.Password.Hash();
        }






    }
}
