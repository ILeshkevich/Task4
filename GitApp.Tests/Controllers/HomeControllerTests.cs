using GitApp.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace GitApp.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> mockLogger;
        private readonly HomeController controller;

        public HomeControllerTests()
        {   
            mockLogger = new Mock<ILogger<HomeController>>();
            controller = new HomeController(mockLogger.Object);
        }
        
        [Fact]
        public void IndexViewIsTypeViewResult()
        {
            // Act
            var result = controller.Index();
            // Assert
            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public void IndexViewNameIsIndex()
        {
            // Act
            var result = controller.Index() as ViewResult;
            // Assert
            Assert.Equal("Index" ,result?.ViewName);
        }
    }
}