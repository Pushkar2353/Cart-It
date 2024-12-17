using Cart_It.Controllers;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cart_It_Testing
{
    [TestFixture]
    public class CartControllerTests
    {
        private Mock<ICartService> _cartServiceMock;
        private Mock<AppDbContext> _dbContextMock;
        private Mock<ILogger<CartController>> _loggerMock;
        private CartController _controller;

        [SetUp]
        public void SetUp()
        {
            _cartServiceMock = new Mock<ICartService>();
            _dbContextMock = new Mock<AppDbContext>();
            _loggerMock = new Mock<ILogger<CartController>>();
            _controller = new CartController(_cartServiceMock.Object, _dbContextMock.Object, _loggerMock.Object);

        }

        [Test]
        public async Task GetCartById_ReturnsOk_WhenCartExists()
        {
            // Arrange
            int cartId = 1;
            var cartDto = new CartDTO { CartId = cartId, CustomerId = 1, ProductId = 1, CartQuantity = 2, Amount = 200 };
            _cartServiceMock.Setup(service => service.GetCartByIdAsync(cartId)).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.GetCartById(cartId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(cartDto, okResult.Value);
        }

        [Test]
        public async Task GetCartById_ReturnsNotFound_WhenCartDoesNotExist()
        {
            // Arrange
            int cartId = 1;
            _cartServiceMock.Setup(service => service.GetCartByIdAsync(cartId)).ReturnsAsync((CartDTO)null);

            // Act
            var result = await _controller.GetCartById(cartId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAllCarts_ReturnsOkWithCarts()
        {
            // Arrange
            var carts = new List<CartDTO>
        {
            new CartDTO { CartId = 1, CustomerId = 1, ProductId = 1, CartQuantity = 1, Amount = 100 },
            new CartDTO { CartId = 2, CustomerId = 2, ProductId = 2, CartQuantity = 2, Amount = 200 }
        };
            _cartServiceMock.Setup(service => service.GetAllCartsAsync()).ReturnsAsync(carts);

            // Act
            var result = await _controller.GetAllCarts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(carts, okResult.Value);
        }

        [Test]
        public async Task AddCart_ReturnsCreatedAtAction_WhenCartIsAdded()
        {
            // Arrange
            var cartDto = new CartDTO
            {
                CartId = 1,
                CustomerId = 1,
                ProductId = 1,
                CartQuantity = 2,
                Amount = 100
            };

            _cartServiceMock.Setup(service => service.AddCartAsync(cartDto))
                .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.AddCart(cartDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult, but got null.");
            Assert.AreEqual(cartDto, createdResult.Value);
        }


        [Test]
        public async Task UpdateCart_ReturnsNoContent_WhenCartIsUpdated()
        {
            // Arrange
            int cartId = 1;
            var cartDto = new CartDTO { CartId = cartId, CustomerId = 1, ProductId = 1, CartQuantity = 1, Amount = 100 };

            var mockProducts = new List<Product>
    {
        new Product { ProductId = 1, ProductPrice = 100 }
    }.AsQueryable();

            var mockProductDbSet = new Mock<DbSet<Product>>();
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(mockProducts.Provider);
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(mockProducts.Expression);
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(mockProducts.ElementType);
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(mockProducts.GetEnumerator());

            _dbContextMock.Setup(db => db.Products).Returns(mockProductDbSet.Object);

           // _cartServiceMock.Setup(service => service.UpdateCartAsync(cartId, cartDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCart(cartId, cartDto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result, $"Expected NoContentResult, but got: {result.GetType()}");

            var noContentResult = result as NoContentResult;
            Assert.NotNull(noContentResult, "Expected NoContentResult but got null");
            Assert.AreEqual(204, noContentResult.StatusCode, "Expected status code 204");
        }






        [Test]
        public async Task DeleteCart_ReturnsNoContent_WhenCartIsDeleted()
        {
            // Arrange
            int cartId = 1;
            _cartServiceMock.Setup(service => service.DeleteCartAsync(cartId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCart(cartId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task AddCart_ReturnsBadRequest_WhenCartDtoIsNull()
        {
            // Arrange
            CartDTO cartDto = null;

            // Act
            var result = await _controller.AddCart(cartDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
