using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using OneWordStory.WebUI.Controllers;
using Extensions;
using OneWordStory.WebUI.Controllers.Adapters;
using OneWordStory.WebUI.Models;

namespace OneWordStory.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        string email = "email@email.com";
        string password = "password";
        
        [Test]
        public void LoginCreatesNewUserIfUserDoesNotExist()
        {
            
            // Setup
            
            Mock<IUserRepository> mockRepository = new Mock<IUserRepository>();
            Mock<IFormsAuthentication> mockAuth = new Mock<IFormsAuthentication>();
            mockRepository.Setup(m => m.GetUserByEmail(It.Is<string>(s => s == email)))
                                .Returns<string>(user => new GetUserResult() 
                                { UserCode = UserErrorCode.EmailNotFoundInRepository,
                                  User = new User() { Email = email, Password = password }
                                });

            mockRepository.Setup(m => m.SaveUser(It.Is<User>(u => u.Email == email &&  u.Password == password )))
                                .Returns<User>(user => UserErrorCode.Success);

            HomeController controller = new HomeController(mockRepository.Object, mockAuth.Object);

            // Act
            var result = controller.Login(new Login() { Email = email, Password = password });
            
            // Assert
            mockRepository.VerifyAll();
            result.AssertRedirectToRouteResult("Index", "Story");

        }

        
        [Test]
        public void LoginReturnsViewErrorIfPasswordDoesNotMatch()
        {
            // Setup
            Mock<IUserRepository> mockRepository = new Mock<IUserRepository>();
            Mock<IFormsAuthentication> mockAuth = new Mock<IFormsAuthentication>();
      
            mockRepository.Setup(m => m.GetUserByEmail(It.Is<string>(s => s == email)))
                                .Returns<string>(user => new GetUserResult() { UserCode = UserErrorCode.Success, User = new User() { Email = email, Password = "thiswontmatch", Id = "users/182736" } });

            HomeController controller = new HomeController(mockRepository.Object, mockAuth.Object);

            // Act
            ActionResult result = controller.Login(new Login() { Email = email, Password = password });

            // Assert
            mockRepository.VerifyAll();
            Assert.IsFalse(result.IsValidModelState());
            Assert.AreEqual(result.GetErrorMessage(0), "Login failed.");
            result.AssertViewResult(controller, "Index");
            
            
        }

        
        [Test]
        public void InvalidModelStateSendUserBackToIndex()
        {
            // Setup
            var login = new Login() { Email = string.Empty, Password = string.Empty };
            Mock<IUserRepository> mockRepository = new Mock<IUserRepository>();
            Mock<IFormsAuthentication> mockAuth = new Mock<IFormsAuthentication>();
       
            HomeController controller = new HomeController(mockRepository.Object, mockAuth.Object);
            controller.ModelState.AddModelError("x", "x");

            // Act
            ActionResult result = controller.Login(login);

            // Assert
            result.AssertViewResult(controller, "Index");
            

        }

        
        [Test]
        public void LoginSetsCookieAndRedirectsIfPasswordsMatch()
        {
            // Setup
            Mock<IUserRepository> mockRepository = new Mock<IUserRepository>();
            Mock<IFormsAuthentication> mockAuth = new Mock<IFormsAuthentication>();
            mockRepository.Setup(m => m.GetUserByEmail(It.Is<string>(s => s == email)))
                                .Returns<string>(user => new GetUserResult() { UserCode = UserErrorCode.Success, User = new User() { Email = email, Password = password.Hash(), Id = "users/182736" } });

            mockAuth.Setup(m => m.SetAuthCookie(email, true));

            HomeController controller = new HomeController(mockRepository.Object, mockAuth.Object);
            

            // Act
            ActionResult result = controller.Login(new Login() { Email = email, Password = password });

            // Assert
            mockRepository.VerifyAll();

            result.AssertRedirectToRouteResult("Index", "Story");

        }

    }
}

