using iranAttractions.Controllers;
using iranAttractions.data;
using iranAttractions.Models;
using iranAttractions.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
namespace Test_iran
{
    public class AccountControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new AccountController(_mockDbContext.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_ValidUser_RedirectsToPrivacy()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                phonenumber = "1234567890",
                password = "password",
                RememberMe = true
            };

            var user = new User
            {
                Phonenumber = "1234567890",
                password = "password",
                UserName = "TestUser"
            };

            _mockDbContext.Setup(db => db.User.SingleOrDefault(It.IsAny<Func<User, bool>>()))
                          .Returns(user);

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthService = new Mock<IAuthenticationService>();
            mockHttpContext.Setup(m => m.RequestServices.GetService(typeof(IAuthenticationService)))
                           .Returns(mockAuthService.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            
            var result = await Task.FromResult(_controller.Login(loginViewModel));

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Privacy", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void Register_InvalidModel_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");

            var registerViewModel = new RegisterViewModel();

            // Act
            var result = _controller.Register(registerViewModel);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthService = new Mock<IAuthenticationService>();
            mockHttpContext.Setup(m => m.RequestServices.GetService(typeof(IAuthenticationService)))
                           .Returns(mockAuthService.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = await Task.FromResult(_controller.Logout());


            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Account/Login", redirectResult.Url);
        }
    }
}
