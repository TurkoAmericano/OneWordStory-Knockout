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
using Facebook;



namespace OneWordStory.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/


        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        private IUserRepository _repository;
        private IFormsAuthentication _authWrapper;

        public HomeController(IUserRepository repository, IFormsAuthentication authWrapper)
        {
            _repository = repository;
            _authWrapper = authWrapper;
            
        }

        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "643310709067274",
                client_secret = "15c2fca2d83cac4f2d4cc116ae6eeadf",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "643310709067274",
                client_secret = "15c2fca2d83cac4f2d4cc116ae6eeadf",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            // TODO: Authenticate User

            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me?fields=first_name,last_name,id,email");

            Login login = new Login() { Email = me.email, Password = Guid.NewGuid().ToString().Replace("-", "") };

            return Login(login);
            

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
                
                result = _repository.SaveUser(new User() { Email = login.Email, Password = login.Password });
                
            }

            if (LoginSuccessful(login, result) || newUser)
            {
                _authWrapper.SetAuthCookie(result.User.Id, true);
                return RedirectToAction("Index", "Story");

            }
            else
            {
                ViewBag.Error = "Login failed";
                return View("Index");
            }



        }

        private static bool LoginSuccessful(Login login, GetUserResult result)
        {
            return result.UserCode == UserErrorCode.Success && result.User.Password == login.Password.Hash();
        }






    }
}
