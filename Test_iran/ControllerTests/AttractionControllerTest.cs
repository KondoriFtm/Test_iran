using iranAttractions.Controllers;
using iranAttractions.data;
using iranAttractions.Models;
using iranAttractions.Services;
using iranAttractions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace iran.Tests
{
    public class AttractionControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private readonly AttractionController _controller;

        public AttractionControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            _controller = new AttractionController(_mockDbContext.Object, _mockHostingEnvironment.Object);
        }

        [Fact]
        public void DisplayInfoes_ReturnsViewResult_WithSightseeingViewModel()
        {
            // Arrange
            var sightseeings = new Sightseeing { Id = 1, Name = "Azadi Tower", CityId = 1, lat = 35.6892, lon = 51.3890 };
            var hotels = new List<Hotel> { new Hotel { Id = 1, Name = "Hotel 1", cityId = 1, lat = 35.6892, lon = 51.3890 } };
            var resturants = new List<Resturant> { new Resturant { Id = 1, Name = "Resturant 1", cityId = 1, lat = 35.6892, lon = 51.3890 } };
            var comments = new List<Comment> { new Comment { Id = 1, Description = "Great place!", SightseeingId = 1, State = 1, Users = new User { Phonenumber = "1234567890" } } };
            var pictures = new List<Picture> { new Picture { Id = 1, FilePath = "path/to/image1.jpg", SightseeingId = 1, state = 1 } };

            _mockDbContext.Setup(db => db.sightseeing.Include(s => s.parts).FirstOrDefault(It.IsAny<Func<Sightseeing, bool>>())).Returns(sightseeings);
            _mockDbContext.Setup(db => db.Hotels.Where(It.IsAny<Func<Hotel, bool>>())).Returns(hotels.AsQueryable());
            _mockDbContext.Setup(db => db.Resturants.Where(It.IsAny<Func<Resturant, bool>>())).Returns(resturants.AsQueryable());
            //_mockDbContext.Setup(db => db.Comment.Where(It.IsAny<Func<Comment, bool>>()).Include(c => c.Users)).Returns(comments.AsQueryable());
            _mockDbContext.Setup(db => db.Pictures.Where(It.IsAny<Func<Picture, bool>>())).Returns(pictures.AsQueryable());

            // Act
            var result = _controller.DisplayInfoes(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SightseeingViewModel>(viewResult.Model);
            Assert.Equal(sightseeings, model.sightseeing);
            Assert.Equal(comments, model.Comments);
            Assert.Equal(pictures, model.Pictures);
        }

        [Fact]
        public void AddPicture_ReturnsRedirectToActionResult()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1234567890")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockHostingEnvironment.Setup(m => m.WebRootPath).Returns("wwwroot");

            // Act
            var result = _controller.AddPicture(1, fileMock.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayInfoes", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public void AddComment_ReturnsRedirectToActionResult()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1234567890")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var sightseeing = new Sightseeing { Id = 1, Name = "Azadi Tower" };
            var userEntity = new User { Phonenumber = "1234567890" };

            _mockDbContext.Setup(db => db.sightseeing.Include(s => s.Comments).ThenInclude(c => c.Users).FirstOrDefault(It.IsAny<Func<Sightseeing, bool>>())).Returns(sightseeing);
            _mockDbContext.Setup(db => db.User.FirstOrDefault(It.IsAny<Func<User, bool>>())).Returns(userEntity);

            // Act
            var result = _controller.AddComment(1, "Great place!", "User1", "path/to/pic.jpg");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayInfoes", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public void Like_ReturnsJsonResult()
        {
            // Arrange
            var picture = new Picture { Id = 1, likecounts = 0 };
            _mockDbContext.Setup(db => db.Pictures.Find(It.IsAny<int>())).Returns(picture);

            // Act
            var result = _controller.Like(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<Dictionary<string, object>>(jsonResult.Value);
            Assert.Equal(1, data["likes"]);
        }

        [Fact]
        public void Unlike_ReturnsJsonResult()
        {
            // Arrange
            var picture = new Picture { Id = 1, likecounts = 1 };
            _mockDbContext.Setup(db => db.Pictures.Find(It.IsAny<int>())).Returns(picture);

            // Act
            var result = _controller.Unlike(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<Dictionary<string, object>>(jsonResult.Value);
            Assert.Equal(0, data["likes"]);
        }
    }
}
