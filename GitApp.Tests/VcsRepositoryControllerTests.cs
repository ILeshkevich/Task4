using GitApp.Controllers;
using GitApp.Models.ViewModels;
using GitApp.Repositories;
using GitApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace GitApp.Tests
{
    public class VcsRepositoryControllerTests
    {
        private readonly Mock<ILogger<VcsRepositoryController>> mockLogger;
        private readonly Mock<IDbVcsRepositoryRepository> mockRepositoryRepository;
        private readonly Mock<IRepositoryWorker> mockRepositoryWorker;
        private readonly VcsRepositoryController controller;

        public VcsRepositoryControllerTests()
        {   
            mockLogger = new Mock<ILogger<VcsRepositoryController>>();
            mockRepositoryRepository = new Mock<IDbVcsRepositoryRepository>();
            mockRepositoryWorker= new Mock<IRepositoryWorker>();
            mockRepositoryRepository.Setup(d => d.FirstOrDefault(0)).Returns(GetFirstRepositoryViewModel());
            controller = new VcsRepositoryController(mockLogger.Object,
                mockRepositoryRepository.Object,
                mockRepositoryWorker.Object);
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
        
        [Fact]
        public void InfoViewIsTypeViewResult()
        {
            // Act
            var result = controller.Info(0);
            // Assert
            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public void InfoViewIsTypeRedirectToActionResult()
        {
            // Act
            var result = controller.Info(1);
            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
        
        [Fact]
        public void InfoViewNameIsInfo()
        {
            // Act
            var result = controller.Info(0) as ViewResult;
            // Assert
            Assert.Equal("Info" ,result?.ViewName);
        }
        
        [Fact]
        public void InfoViewModelExist()
        {
            // Act
            var result = controller.Info(0) as ViewResult;
            // Assert
            Assert.NotNull(result?.Model);
        }

        private VcsRepositoryViewModel GetFirstRepositoryViewModel() => new VcsRepositoryViewModel();

    }
}