using AutoMapper;
using Cart_It.Controllers;
using Cart_It.DTOs;
using Cart_It.Models;
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
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private Mock<ILogger<ProductController>> _mockLogger;
        private ProductController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllProducts_ReturnsOk_WithListOfProducts()
        {
            // Arrange
            var products = new List<ProductDTO>
        {
            new ProductDTO { ProductId = 1, ProductName = "Product1", ProductDescription = "Description1", ProductPrice = 10.5M },
            new ProductDTO { ProductId = 2, ProductName = "Product2", ProductDescription = "Description2", ProductPrice = 20.0M }
        };
            _mockProductService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(products.Count, (okResult.Value as List<ProductDTO>)?.Count);
        }

        [Test]
        public async Task GetProductById_ReturnsOk_WithProduct()
        {
            // Arrange
            int productId = 1;
            var product = new ProductDTO { ProductId = productId, ProductName = "Product1", ProductDescription = "Description1", ProductPrice = 15.0M };
            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            var returnedProduct = okResult.Value as ProductDTO;
            Assert.AreEqual(productId, returnedProduct?.ProductId);
        }

        [Test]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;
            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync((ProductDTO)null);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var response = JObject.FromObject(notFoundResult.Value);
            Assert.AreEqual("Product not found", response["message"].ToString());
        }

        [Test]
        public async Task AddProduct_ReturnsCreated_WhenProductIsAddedSuccessfully()
        {
            // Arrange
            var newProduct = new ProductDTO
            {
                ProductId = 1,
                ProductName = "Product1",
                ProductDescription = "Description1",
                ProductPrice = 15.0M,
                ProductStock = 10
            };

            _mockProductService.Setup(service => service.AddProductAsync(It.IsAny<ProductDTO>()))
                .ReturnsAsync(newProduct);

            // Act
            var result = await _controller.AddProduct(newProduct);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsNotNull(createdResult.Value);
            var returnedProduct = createdResult.Value as ProductDTO;
            Assert.AreEqual(newProduct.ProductId, returnedProduct?.ProductId);
        }

        [Test]
        public async Task AddProduct_ReturnsBadRequest_WhenProductDataIsInvalid()
        {
            // Arrange
            ProductDTO nullProduct = null;

            // Act
            var result = await _controller.AddProduct(nullProduct);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var response = JObject.FromObject(badRequestResult.Value);
            Assert.AreEqual("Invalid product data.", response["message"].ToString());
        }

        [Test]
        public async Task UpdateProduct_ReturnsOk_WhenProductIsUpdatedSuccessfully()
        {
            // Arrange
            int productId = 1;
            var updatedProduct = new ProductDTO
            {
                ProductId = productId,
                ProductName = "UpdatedProduct",
                ProductDescription = "UpdatedDescription",
                ProductPrice = 25.0M,
                ProductStock = 20
            };

            _mockProductService.Setup(service => service.UpdateProductAsync(productId, It.IsAny<ProductDTO>()))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.UpdateProduct(productId, updatedProduct);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            var returnedProduct = okResult.Value as ProductDTO;
            Assert.AreEqual(updatedProduct.ProductName, returnedProduct?.ProductName);
        }

        [Test]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeletedSuccessfully()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(service => service.DeleteProductAsync(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;
            _mockProductService.Setup(service => service.DeleteProductAsync(productId))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var response = JObject.FromObject(notFoundResult.Value);
            Assert.AreEqual("Product not found", response["message"].ToString());
        }
    }

}
