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
    public class CategoryControllerTests
    {
        private Mock<ICategoryService> _mockCategoryService;
        private Mock<ILogger<CategoryController>> _mockLogger;
        private CategoryController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_mockCategoryService.Object, _mockLogger.Object);
        }

        // Test GetCategories() - Successful case
        [Test]
        public async Task GetCategories_ReturnsOkResult_WithCategories()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = 1, CategoryName = "Electronics" },
                new CategoryDTO { CategoryId = 2, CategoryName = "Furniture" }
            };
            _mockCategoryService.Setup(service => service.GetAllCategoriesAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnCategories = okResult.Value as IEnumerable<CategoryDTO>;
            Assert.AreEqual(categories.Count(), returnCategories.Count());
        }

        // Test GetCategories() - Error case
        [Test]
        public async Task GetCategories_ReturnsStatusCode500_WhenExceptionOccurs()
        {
            // Arrange
            _mockCategoryService.Setup(service => service.GetAllCategoriesAsync()).ThrowsAsync(new Exception("Error fetching categories"));

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        // Test GetCategoryById() - Successful case
        [Test]
        public async Task GetCategoryById_ReturnsOkResult_WithCategory()
        {
            // Arrange
            var category = new CategoryDTO { CategoryId = 1, CategoryName = "Electronics" };
            _mockCategoryService.Setup(service => service.GetCategoryByIdAsync(1)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetCategoryById(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnCategory = okResult.Value as CategoryDTO;
            Assert.AreEqual(category.CategoryId, returnCategory.CategoryId);
        }

        // Test GetCategoryById() - Category Not Found
        [Test]
        public async Task GetCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int nonExistentCategoryId = 999; // assuming this ID doesn't exist in the database

            // Set up mock service to return null for the non-existent category
            _mockCategoryService.Setup(service => service.GetCategoryByIdAsync(nonExistentCategoryId))
                .ReturnsAsync((CategoryDTO)null);

            // Act
            var result = await _controller.GetCategoryById(nonExistentCategoryId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            // Use JObject to parse and inspect the response structure
            var response = JObject.FromObject(notFoundResult.Value);

            Assert.IsNotNull(response["message"]);
            Assert.AreEqual("Category not found", response["message"].ToString());
        }



        // Test CreateCategory() - Successful case
        [Test]
        public async Task CreateCategory_ReturnsCreatedAtActionResult_WhenCategoryIsCreated()
        {
            // Arrange
            var categoryDto = new CategoryDTO { CategoryName = "Electronics" };
            var createdCategory = new CategoryDTO { CategoryId = 1, CategoryName = "Electronics" };
            _mockCategoryService.Setup(service => service.AddCategoryAsync(categoryDto)).ReturnsAsync(createdCategory);

            // Act
            var result = await _controller.CreateCategory(categoryDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.AreEqual("GetCategoryById", createdAtActionResult.ActionName);
            Assert.AreEqual(createdCategory.CategoryId, createdAtActionResult.RouteValues["id"]);
        }


        // Test UpdateCategory() - Successful case
        [Test]
        public async Task UpdateCategory_ReturnsOkResult_WhenCategoryIsUpdated()
        {
            // Arrange
            var categoryDto = new CategoryDTO { CategoryId = 1, CategoryName = "Updated Electronics" };
            var updatedCategory = new CategoryDTO { CategoryId = 1, CategoryName = "Updated Electronics" };
            _mockCategoryService.Setup(service => service.UpdateCategoryAsync(1, categoryDto)).ReturnsAsync(updatedCategory);

            // Act
            var result = await _controller.UpdateCategory(1, categoryDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnCategory = okResult.Value as CategoryDTO;
            Assert.AreEqual(updatedCategory.CategoryName, returnCategory.CategoryName);
        }

        // Test DeleteCategory() - Successful case
        [Test]
        public async Task DeleteCategory_ReturnsNoContent_WhenCategoryIsDeleted()
        {
            // Arrange
            _mockCategoryService.Setup(service => service.DeleteCategoryAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCategory(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        // Test DeleteCategory() - Not Found when category does not exist
        [Test]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int nonExistentCategoryId = 999; // assuming this ID doesn't exist in the database
            var expectedMessage = "Category not found";

            // Set up mock service to throw KeyNotFoundException for the non-existent category
            _mockCategoryService.Setup(service => service.DeleteCategoryAsync(nonExistentCategoryId))
                .ThrowsAsync(new KeyNotFoundException(expectedMessage));

            // Act
            var result = await _controller.DeleteCategory(nonExistentCategoryId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            // Use JObject to parse and inspect the response structure
            var response = JObject.FromObject(notFoundResult.Value);

            Assert.IsNotNull(response["message"]);
            Assert.AreEqual(expectedMessage, response["message"].ToString());
        }

    }
}
