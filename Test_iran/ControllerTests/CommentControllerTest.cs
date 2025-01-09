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
    public class CommentsControllerTests
    {
        private readonly Mock<MyDbContext> _mockDbContext;
        private readonly CommentsController _controller;

        public CommentsControllerTests()
        {
            _mockDbContext = new Mock<MyDbContext>();
            _controller = new CommentsController(_mockDbContext.Object);
        }

        [Fact]
        public void DisplayComments_ReturnsViewResult_WithCommentsList()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment { Id = 1, Description = "Great place!", State = 0, Sightseeings = new Sightseeing { Id = 1, Name = "Azadi Tower", City = new City { Id = 1, Name = "Tehran" } }, Users = new User { Phonenumber = "1234567890" } }
            };

            var mockCommentsDbSet = new Mock<DbSet<Comment>>();
            mockCommentsDbSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(comments.AsQueryable().Provider);
            mockCommentsDbSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(comments.AsQueryable().Expression);
            mockCommentsDbSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(comments.AsQueryable().ElementType);
            mockCommentsDbSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(comments.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Comment).Returns(mockCommentsDbSet.Object);

            // Act
            var result = _controller.DisplayComments();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Comment>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(comments[0].Description, model[0].Description);
        }

        [Fact]
        public void DeleteComments_ReturnsRedirectToActionResult()
        {
            // Arrange
            var comment = new Comment { Id = 1, State = 0 };
            _mockDbContext.Setup(db => db.Comment.Find(It.IsAny<int>())).Returns(comment);

            // Act
            var result = _controller.DeleteComments(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayComments", redirectResult.ActionName);
            Assert.Equal(2, comment.State);
        }

        [Fact]
        public void Edit_ReturnsViewResult_WithComment()
        {
            // Arrange
            var comment = new Comment { Id = 1, Description = "Great place!", Sightseeings = new Sightseeing { Id = 1, Name = "Azadi Tower", City = new City { Id = 1, Name = "Tehran" } }, Users = new User { Phonenumber = "1234567890" } };
            _mockDbContext.Setup(db => db.Comment.Include(s => s.Sightseeings).ThenInclude(c => c.City).Include(u => u.Users).FirstOrDefault(It.IsAny<Func<Comment, bool>>())).Returns(comment);

            // Act
            var result = _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Comment>(viewResult.Model);
            Assert.Equal(comment.Description, model.Description);
        }

        [Fact]
        public void EditComments_ReturnsRedirectToActionResult()
        {
            // Arrange
            var comment = new Comment { Id = 1, Description = "Great place!" };
            var updatedComment = new Comment { Id = 1, Description = "Updated description" };
            _mockDbContext.Setup(db => db.Comment.Include(s => s.Sightseeings).ThenInclude(c => c.City).Include(u => u.Users).FirstOrDefault(It.IsAny<Func<Comment, bool>>())).Returns(comment);

            // Act
            var result = _controller.EditComments(updatedComment);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayComments", redirectResult.ActionName);
            Assert.Equal(updatedComment.Description, comment.Description);
        }

        [Fact]
        public void ConfirmComments_ReturnsRedirectToActionResult()
        {
            // Arrange
            var comment = new Comment { Id = 1, State = 0 };
            _mockDbContext.Setup(db => db.Comment.Find(It.IsAny<int>())).Returns(comment);

            // Act
            var result = _controller.ConfirmComments(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DisplayComments", redirectResult.ActionName);
            Assert.Equal(1, comment.State);
        }
    }
}
