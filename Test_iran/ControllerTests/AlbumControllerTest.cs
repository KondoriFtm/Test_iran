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
    public class AlbumControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly AlbumController _controller;

        public AlbumControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new AlbumController(_mockDbContext.Object);
        }

        [Fact]
        public void PictureList_ReturnsViewResult_WithPictureListViewModel()
        {
            // Arrange
            var cities = new List<City> { new City { Id = 1, Name = "Tehran" } };
            var sightseeings = new List<Sightseeing> { new Sightseeing { Id = 1, Name = "Azadi Tower", City = cities[0] } };
            var pictures = new List<Picture>
            {
                new Picture { Id = 1, FilePath = "path/to/image1.jpg", sightseeing = sightseeings[0], dateImported = DateTime.Now }
            };

            var mockPicturesDbSet = new Mock<DbSet<Picture>>();
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.Provider).Returns(pictures.AsQueryable().Provider);
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.Expression).Returns(pictures.AsQueryable().Expression);
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.ElementType).Returns(pictures.AsQueryable().ElementType);
            mockPicturesDbSet.As<IQueryable<Picture>>().Setup(m => m.GetEnumerator()).Returns(pictures.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Pictures).Returns(mockPicturesDbSet.Object);

            // Act
            var result = _controller.PictureList("Tehran");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<PictureListViewModel>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(pictures[0].Id, model[0].PictureId);
            Assert.Equal(pictures[0].FilePath, model[0].FilePath);
            Assert.Equal(pictures[0].sightseeing.Name, model[0].SightseeingName);
            Assert.Equal(pictures[0].sightseeing.City.Name, model[0].ProvinceName);
            Assert.Equal(pictures[0].dateImported, model[0].DateImported);
        }
    }
}
