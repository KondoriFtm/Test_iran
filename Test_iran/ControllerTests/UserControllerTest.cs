using iranAttractions.Controllers;
using iranAttractions.data;
using iranAttractions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iran.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new UserController(_mockDbContext.Object);
        }

        [Fact]
        public void DisplayUsers_ReturnsViewResult_WithUsersList()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Phonenumber = "1234567890", UserName = "John Doe" }
            };

            var mockUserDbSet = new Mock<DbSet<User>>();
            mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.AsQueryable().Provider);
            mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockUserDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockUserDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.User).Returns(mockUserDbSet.Object);

            // Act
            var result = _controller.DisplayUsers();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<User>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(users[0].Phonenumber, model[0].Phonenumber);
        }

        [Fact]
        public void DeleteUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var user = new User { Phonenumber = "1234567890", UserName = "John Doe" };
            _mockDbContext.Setup(db => db.User.SingleOrDefault(It.IsAny<Func<User, bool>>())).Returns(user);

            // Act
            var result = _controller.DeleteUSer("1234567890");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayUsers", redirectResult.ActionName);
            _mockDbContext.Verify(db => db.User.Remove(user), Times.Once);
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void SearchUser_ReturnsRedirectToActionResult()
        {
            // Act
            var result = _controller.SearrchUser();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
