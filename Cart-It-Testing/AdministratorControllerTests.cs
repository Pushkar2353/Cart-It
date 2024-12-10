using AutoMapper;
using Cart_It.Controllers;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart_It_Testing
{
    [TestFixture]
    public class AdministratorControllerTests
    {
        private Mock<IAdministratorService> _mockAdministratorService;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<AdministratorController>> _mockLogger;
        private AdministratorController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAdministratorService = new Mock<IAdministratorService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AdministratorController>>();
            _controller = new AdministratorController(_mockAdministratorService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAdministratorById_ShouldReturnAdministrator_WhenAdministratorExists()
        {
            // Arrange
            int adminId = 1;
            var adminDto = new AdministratorDTO { AdminId = adminId, Email = "admin@example.com" };
            _mockAdministratorService.Setup(s => s.GetAdministratorByIdAsync(adminId)).ReturnsAsync(adminDto);

            // Act
            var result = await _controller.GetAdministratorById(adminId);

            // Assert
            var actionResult = result as ActionResult<AdministratorDTO>;
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(adminDto, okResult.Value);
        }

        [Test]
        public async Task GetAdministratorById_ShouldReturnNotFound_WhenAdministratorDoesNotExist()
        {
            // Arrange
            int adminId = 999; // Non-existing ID
            _mockAdministratorService.Setup(s => s.GetAdministratorByIdAsync(adminId)).ReturnsAsync((AdministratorDTO)null);

            // Act
            var result = await _controller.GetAdministratorById(adminId);

            // Assert
            var actionResult = result as ActionResult<AdministratorDTO>;
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetAllAdministrators_ShouldReturnAllAdministrators()
        {
            // Arrange
            var adminList = new List<AdministratorDTO>
        {
            new AdministratorDTO { AdminId = 1, Email = "admin1@example.com" },
            new AdministratorDTO { AdminId = 2, Email = "admin2@example.com" }
        };
            _mockAdministratorService.Setup(s => s.GetAllAdministratorsAsync()).ReturnsAsync(adminList);

            // Act
            var result = await _controller.GetAllAdministrators();

            // Assert
            var actionResult = result as ActionResult<IEnumerable<AdministratorDTO>>;
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(adminList, okResult.Value);
        }

        [Test]
        public async Task CreateAdministrator_ShouldReturnCreatedAdministrator()
        {
            // Arrange
            var adminDto = new AdministratorDTO
            {
                Email = "newadmin@example.com",
                Password = "Password123!"
            };
            var createdAdmin = new AdministratorDTO { AdminId = 1, Email = "newadmin@example.com" };
            _mockAdministratorService.Setup(s => s.CreateAdministratorAsync(adminDto)).ReturnsAsync(createdAdmin);

            // Act
            var result = await _controller.CreateAdministrator(adminDto);

            // Assert
            var actionResult = result as ActionResult<AdministratorDTO>;
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(createdAdmin, createdResult.Value);
        }

        [Test]
        public async Task UpdateAdministrator_ShouldReturnUpdatedAdministrator_WhenAdministratorExists()
        {
            // Arrange
            int adminId = 1;
            var adminDto = new AdministratorDTO { AdminId = adminId, Email = "updatedadmin@example.com" };
            _mockAdministratorService.Setup(s => s.UpdateAdministratorAsync(adminId, adminDto)).ReturnsAsync(adminDto);

            // Act
            var result = await _controller.UpdateAdministrator(adminId, adminDto);

            // Assert
            var actionResult = result as ActionResult<AdministratorDTO>;
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(adminDto, okResult.Value);
        }

        [Test]
        public async Task UpdateAdministrator_ShouldReturnNotFound_WhenAdministratorDoesNotExist()
        {
            // Arrange
            int adminId = 999; // Non-existing admin
            var adminDto = new AdministratorDTO { AdminId = adminId, Email = "updatedadmin@example.com" };
            _mockAdministratorService.Setup(s => s.UpdateAdministratorAsync(adminId, adminDto)).ReturnsAsync((AdministratorDTO)null);

            // Act
            var result = await _controller.UpdateAdministrator(adminId, adminDto);

            // Assert
            var actionResult = result as ActionResult<AdministratorDTO>;
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task DeleteAdministrator_ShouldReturnNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            int adminId = 1;
            _mockAdministratorService.Setup(s => s.DeleteAdministratorAsync(adminId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAdministrator(adminId);

            // Assert
            var actionResult = result as ActionResult;
            Assert.IsInstanceOf<NoContentResult>(actionResult);
        }

        [Test]
        public async Task DeleteAdministrator_ShouldReturnNotFound_WhenAdministratorDoesNotExist()
        {
            // Arrange
            int adminId = 999; // Non-existing admin
            _mockAdministratorService.Setup(s => s.DeleteAdministratorAsync(adminId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAdministrator(adminId);

            // Assert
            var actionResult = result as ActionResult;
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
