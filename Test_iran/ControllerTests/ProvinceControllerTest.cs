using iranAttractions.Controllers;
using iranAttractions.data;
using iranAttractions.Models;
using iranAttractions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iran.Tests
{
    public class ProvinceControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly ProvinceController _controller;

        public ProvinceControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new ProvinceController(_mockDbContext.Object);
        }

        [Fact]
        public void DisplayProvince_ReturnsViewResult_WithProvinceViewModel()
        {
            // Arrange
            var province = new City
            {
                Id = 1,
                Name = "Tehran",
                Description = "Capital of Iran",
                sightseeings = new List<Sightseeing> { new Sightseeing { Id = 1, Name = "Azadi Tower", CityId = 1 } },
                hotels = new List<Hotel> { new Hotel { Id = 1, Name = "Hotel 1", cityId = 1 } },
                resturants = new List<Resturant> { new Resturant { Id = 1, Name = "Resturant 1", cityId = 1 } }
            };

            var mockCityDbSet = new Mock<DbSet<City>>();
            mockCityDbSet.As<IQueryable<City>>().Setup(m => m.Provider).Returns(new List<City> { province }.AsQueryable().Provider);
            mockCityDbSet.As<IQueryable<City>>().Setup(m => m.Expression).Returns(new List<City> { province }.AsQueryable().Expression);
            mockCityDbSet.As<IQueryable<City>>().Setup(m => m.ElementType).Returns(new List<City> { province }.AsQueryable().ElementType);
            mockCityDbSet.As<IQueryable<City>>().Setup(m => m.GetEnumerator()).Returns(new List<City> { province }.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.City).Returns(mockCityDbSet.Object);

            // Act
            var result = _controller.DisplayProvince(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProvinceViewModel>(viewResult.Model);
            Assert.Equal(province.Name, model.ProvinceName);
            Assert.Equal(province.Description, model.ProvinceDescription);
            Assert.Equal(province.sightseeings, model.Sightseeings);
            Assert.Equal(province, model.City);
        }
    }
}
