using iranAttractions.Controllers;
using iranAttractions.data;
using iranAttractions.Models;
using iranAttractions.ViewModels;
using iranAttractions.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iran.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new HomeController(_mockLogger.Object, _mockDbContext.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithMainViewModel()
        {
            // Arrange
            var cities = new List<City> { new City { Id = 1, Name = "Tehran" } };
            var sightseeings = new List<Sightseeing> { new Sightseeing { Id = 1, Name = "Azadi Tower", City = cities[0] } };
            _mockDbContext.Setup(db => db.City.ToList()).Returns(cities);
            _mockDbContext.Setup(db => db.sightseeing.ToList()).Returns(sightseeings);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MainViewModel>(viewResult.Model);
            Assert.Equal(cities, model.Cities);
            Assert.Equal(sightseeings, model.sightseeings);
        }

        [Fact]
        public void Privacy_ReturnsViewResult_WithMainViewModel()
        {
            // Arrange
            var cities = new List<City> { new City { Id = 1, Name = "Tehran" } };
            var sightseeings = new List<Sightseeing> { new Sightseeing { Id = 1, Name = "Azadi Tower", City = cities[0] } };
            _mockDbContext.Setup(db => db.City.ToList()).Returns(cities);
            _mockDbContext.Setup(db => db.sightseeing.ToList()).Returns(sightseeings);

            // Act
            var result = _controller.privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MainViewModel>(viewResult.Model);
            Assert.Equal(cities, model.Cities);
            Assert.Equal(sightseeings, model.sightseeings);
        }

        [Fact]
        public void DisplayBoxes_ReturnsViewResult_WithMainViewModel()
        {
            // Arrange
            var cities = new List<City> { new City { Id = 1, Name = "Tehran" } };
            var sightseeings = new List<Sightseeing> { new Sightseeing { Id = 1, Name = "Azadi Tower", City = cities[0] } };
            _mockDbContext.Setup(db => db.City.ToList()).Returns(cities);
            _mockDbContext.Setup(db => db.sightseeing.ToList()).Returns(sightseeings);

            // Act
            var result = _controller.DisplayBoxes();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MainViewModel>(viewResult.Model);
            Assert.Equal(cities, model.Cities);
            Assert.Equal(sightseeings, model.sightseeings);
        }

        [Fact]
        public void SearchResultCity_ReturnsRedirectToActionResult_WhenCityFound()
        {
            // Arrange
            var city = new City { Id = 1, Name = "Tehran" };
            _mockDbContext.Setup(db => db.City.SingleOrDefault(It.IsAny<Func<City, bool>>())).Returns(city);

            // Act
            var result = _controller.SearchResultCity("Tehran");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayProvince", redirectResult.ActionName);
            Assert.Equal("Province", redirectResult.ControllerName);
            Assert.Equal(city.Id, redirectResult.RouteValues["ItemId"]);
        }

        [Fact]
        public void SearchResultCity_ReturnsRedirectToActionResult_WhenCityNotFound()
        {
            // Arrange
            _mockDbContext.Setup(db => db.City.SingleOrDefault(It.IsAny<Func<City, bool>>())).Returns((City)null);

            // Act
            var result = _controller.SearchResultCity("UnknownCity");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SearchResultSightseeing", redirectResult.ActionName);
            Assert.Equal("UnknownCity", redirectResult.RouteValues["query"]);
        }

        [Fact]
        public void SearchResultSightseeing_ReturnsRedirectToActionResult_WhenSightseeingFound()
        {
            // Arrange
            var sightseeing = new Sightseeing { Id = 1, Name = "Azadi Tower", City = new City { Name = "Tehran" } };
            _mockDbContext.Setup(db => db.sightseeing.Include(s => s.City).SingleOrDefault(It.IsAny<Func<Sightseeing, bool>>())).Returns(sightseeing);

            // Act
            var result = _controller.SearchResultSightseeing("Tehran");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayInfoes", redirectResult.ActionName);
            Assert.Equal("Attraction", redirectResult.ControllerName);
            Assert.Equal(sightseeing.Id, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public void SearchResultSightseeing_ReturnsContentResult_WhenSightseeingNotFound()
        {
            // Arrange
            _mockDbContext.Setup(db => db.sightseeing.Include(s => s.City).SingleOrDefault(It.IsAny<Func<Sightseeing, bool>>())).Returns((Sightseeing)null);

            // Act
            var result = _controller.SearchResultSightseeing("UnknownSightseeing");

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("نتیجه ای یافت نشد", contentResult.Content);
        }

        [Fact]
        public void Search_ReturnsViewResult()
        {
            // Act
            var result = _controller.Search();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
        }
    }
}
