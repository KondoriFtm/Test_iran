using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iranAttractions.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace iran.Tests
{
    public class AdminControllerTests
    {
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _controller = new AdminController();
        }

        [Fact]
        public void Admin_ReturnsViewResult()
        {
            // Act
            var result = _controller.admin();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}

