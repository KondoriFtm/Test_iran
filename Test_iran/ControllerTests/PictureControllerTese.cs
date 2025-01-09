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
    public class PicturesControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly PicturesController _controller;

        public PicturesControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new PicturesController(_mockDbContext.Object);
        }

        [Fact]
        public void DisplayPictures_ReturnsViewResult_WithPicturesList()
        {
            // Arrange
            var pictures = new List<Picture>
            {
                new Picture { Id = 1, FilePath = "path/to/image1.jpg", state = 0, sightseeing = new Sightseeing { Id = 1, Name = "Azadi Tower", City = new City { Id = 1, Name = "Tehran" } } }
            };

            var mockPicturesDbSet = new Mock<DbSet<Picture>>();
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.Provider).Returns(pictures.AsQueryable().Provider);
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.Expression).Returns(pictures.AsQueryable().Expression);
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.ElementType).Returns(pictures.AsQueryable().ElementType);
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.GetEnumerator()).Returns(pictures.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Pictures).Returns(mockPicturesDbSet.Object);

            // Act
            var result = _controller.DisplayPictures();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Picture>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(pictures[0].FilePath, model[0].FilePath);
        }

        [Fact]
        public void Confirm_pic_ReturnsRedirectToActionResult()
        {
            // Arrange
            var picture = new Picture { Id = 1, state = 0 };
            _mockDbContext.Setup(db => db.Pictures.Find(It.IsAny<int>())).Returns(picture);

            // Act
            var result = _controller.Confirm_pic(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayPictures", redirectResult.ActionName);
            Assert.Equal(1, picture.state);
        }

        [Fact]
        public void Delete_pic_ReturnsRedirectToActionResult()
        {
            // Arrange
            var picture = new Picture { Id = 1, state = 0 };
            _mockDbContext.Setup(db => db.Pictures.Find(It.IsAny<int>())).Returns(picture);

            // Act
            var result = _controller.Delete_pic(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayPictures", redirectResult.ActionName);
            Assert.Equal(2, picture.state);
        }
    }
}

