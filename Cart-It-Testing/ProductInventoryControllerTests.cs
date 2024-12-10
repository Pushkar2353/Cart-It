using Cart_It.Controllers;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart_It_Testing
{
    [TestFixture]
    public class ProductInventoryControllerTests
    {
        private Mock<IProductInventoryService> _mockInventoryService;
        private Mock<ILogger<ProductInventoryController>> _mockLogger;
        private ProductInventoryController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockInventoryService = new Mock<IProductInventoryService>();
            _mockLogger = new Mock<ILogger<ProductInventoryController>>();
            _controller = new ProductInventoryController(_mockInventoryService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AddInventory_ReturnsCreated_WhenInventoryIsAddedSuccessfully()
        {
            // Arrange
            var inventoryDto = new ProductInventoryDTO
            {
                ProductId = 1,
                CurrentStock = 100,
                MinimumStock = 20,
                LastRestockDate = DateTime.Now.AddDays(-10),
                NextRestockDate = DateTime.Now.AddDays(10)
            };
            var addedInventory = new ProductInventoryDTO
            {
                InventoryId = 1,
                ProductId = inventoryDto.ProductId,
                CurrentStock = inventoryDto.CurrentStock,
                MinimumStock = inventoryDto.MinimumStock,
                LastRestockDate = inventoryDto.LastRestockDate,
                NextRestockDate = inventoryDto.NextRestockDate
            };
            _mockInventoryService.Setup(service => service.AddInventoryAsync(It.IsAny<ProductInventoryDTO>())).ReturnsAsync(addedInventory);

            // Act
            var result = await _controller.AddInventory(inventoryDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsNotNull(createdResult.Value);
            var returnedInventory = createdResult.Value as ProductInventoryDTO;
            Assert.AreEqual(addedInventory.InventoryId, returnedInventory?.InventoryId);
        }

        [Test]
        public async Task AddInventory_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var inventoryDto = new ProductInventoryDTO
            {
                ProductId = 1,
                CurrentStock = -100,
                MinimumStock = 20,
                LastRestockDate = DateTime.Now.AddDays(-10),
                NextRestockDate = DateTime.Now.AddDays(10)
            };
            _mockInventoryService.Setup(service => service.AddInventoryAsync(It.IsAny<ProductInventoryDTO>()))
                .ThrowsAsync(new InvalidOperationException("Invalid inventory data"));

            // Act
            var result = await _controller.AddInventory(inventoryDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var response = JObject.FromObject(badRequestResult.Value);
            Assert.AreEqual("Invalid inventory data", response["message"].ToString());
        }

        [Test]
        public async Task UpdateInventory_ReturnsOk_WhenInventoryIsUpdatedSuccessfully()
        {
            // Arrange
            var inventoryDto = new ProductInventoryDTO
            {
                InventoryId = 1,
                ProductId = 1,
                CurrentStock = 150,
                MinimumStock = 30,
                LastRestockDate = DateTime.Now.AddDays(-5),
                NextRestockDate = DateTime.Now.AddDays(15)
            };
            _mockInventoryService.Setup(service => service.UpdateInventoryAsync(It.IsAny<ProductInventoryDTO>())).ReturnsAsync(inventoryDto);

            // Act
            var result = await _controller.UpdateInventory(inventoryDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            var updatedInventory = okResult.Value as ProductInventoryDTO;
            Assert.AreEqual(inventoryDto.CurrentStock, updatedInventory?.CurrentStock);
        }

        [Test]
        public async Task UpdateInventory_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var inventoryDto = new ProductInventoryDTO
            {
                InventoryId = 1,
                ProductId = 1,
                CurrentStock = 150,
                MinimumStock = 30,
                LastRestockDate = DateTime.Now.AddDays(-5),
                NextRestockDate = DateTime.Now.AddDays(15)
            };
            _mockInventoryService.Setup(service => service.UpdateInventoryAsync(It.IsAny<ProductInventoryDTO>()))
                .ThrowsAsync(new InvalidOperationException("Invalid inventory data"));

            // Act
            var result = await _controller.UpdateInventory(inventoryDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var response = JObject.FromObject(badRequestResult.Value);
            Assert.AreEqual("Invalid inventory data", response["message"].ToString());
        }

        [Test]
        public async Task GetInventoryById_ReturnsOk_WhenInventoryExists()
        {
            // Arrange
            int inventoryId = 1;
            var inventory = new ProductInventoryDTO
            {
                InventoryId = inventoryId,
                ProductId = 1,
                CurrentStock = 100,
                MinimumStock = 20,
                LastRestockDate = DateTime.Now.AddDays(-10),
                NextRestockDate = DateTime.Now.AddDays(10)
            };
            _mockInventoryService.Setup(service => service.GetInventoryByIdAsync(inventoryId)).ReturnsAsync(inventory);

            // Act
            var result = await _controller.GetInventoryById(inventoryId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            var returnedInventory = okResult.Value as ProductInventoryDTO;
            Assert.AreEqual(inventoryId, returnedInventory?.InventoryId);
        }

        [Test]
        public async Task GetInventoryById_ReturnsNotFound_WhenInventoryDoesNotExist()
        {
            // Arrange
            int inventoryId = 999;
            _mockInventoryService.Setup(service => service.GetInventoryByIdAsync(inventoryId)).ReturnsAsync((ProductInventoryDTO)null);

            // Act
            var result = await _controller.GetInventoryById(inventoryId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var response = JObject.FromObject(notFoundResult.Value);
            Assert.AreEqual("Inventory not found", response["message"].ToString());
        }

        [Test]
        public async Task GetInventoryById_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int inventoryId = 1;
            _mockInventoryService.Setup(service => service.GetInventoryByIdAsync(inventoryId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetInventoryById(inventoryId);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.IsNotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);

            var response = JObject.FromObject(serverErrorResult.Value);
            Assert.AreEqual("Unexpected error", response["message"].ToString());
        }
    }
}
