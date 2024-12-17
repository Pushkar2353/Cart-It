using AutoMapper;
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
    public class OrderControllerTests
    {
        private Mock<IOrderService> _orderServiceMock;
        private Mock<AppDbContext> _dbContextMock;
        private Mock<ILogger<OrderController>> _loggerMock;
        private OrderController _controller;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _dbContextMock = new Mock<AppDbContext>();
            _loggerMock = new Mock<ILogger<OrderController>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new OrderController(_orderServiceMock.Object, _mapperMock.Object, _loggerMock.Object, _dbContextMock.Object);
        }

        [Test]
        public async Task AddOrder_ReturnsCreatedAtAction_WhenOrderIsAdded()
        {
            // Arrange
            var orderDto = new OrderDTO
            {
                OrderId = 1,
                CustomerId = 1,
                ProductId = 1,
                ItemQuantity = 2,
                UnitPrice = 100, // Price will be set by the mocked Product
                ShippingAddress = "123 Test St",
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
            };

            var order = new Order
            {
                OrderId = 1,
                CustomerId = 1,
                ProductId = 1,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalAmount = 200
            };

            // Mock the service to return an order when AddOrderAsync is called
            _orderServiceMock.Setup(service => service.AddOrderAsync(orderDto)).ReturnsAsync(order);

            // Mock AutoMapper to return the DTO after mapping
            _mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>())).Returns(orderDto);

            // Act
            var result = await _controller.AddOrder(orderDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult, "Expected CreatedAtActionResult, but got null.");

            // Assert that the action name is "GetOrder"
            Assert.AreEqual(nameof(OrderController.GetOrder), createdAtActionResult?.ActionName);

            // Assert that the returned value is the correct OrderDTO
            var returnedOrder = createdAtActionResult?.Value as OrderDTO;
            Assert.IsNotNull(returnedOrder, "Expected OrderDTO, but got null.");

            // Assert that the returned order has the correct ID and TotalAmount
            Assert.AreEqual(orderDto.OrderId, returnedOrder?.OrderId);
            Assert.AreEqual(orderDto.TotalAmount, returnedOrder?.TotalAmount);
        }



        [Test]
        public async Task AddOrder_ReturnsBadRequest_WhenOrderDtoIsNull()
        {
            // Arrange
            OrderDTO orderDto = null;

            // Act
            var result = await _controller.AddOrder(orderDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task AddOrder_ReturnsBadRequest_WhenProductNotFound()
        {
            // Arrange
            var orderDto = new OrderDTO
            {
                OrderId = 1,
                CustomerId = 1,
                ProductId = 1, // Non-existent product ID (simulate product not found)
                ItemQuantity = 2,
                UnitPrice = 100,
                ShippingAddress = "123 Test St",
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
            };

            // Simulate that the product is not found by setting up the service to throw an exception
            _orderServiceMock.Setup(service => service.AddOrderAsync(orderDto))
                .ThrowsAsync(new InvalidOperationException("Product not found"));

            // Act
            var result = await _controller.AddOrder(orderDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult, but got null.");

            // Access the message properly from the dynamic object
            dynamic value = badRequestResult?.Value;
            Assert.AreEqual("Product not found", value?.message);
        }


        [Test]
        public async Task UpdateOrder_ReturnsOk_WhenOrderIsUpdated()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = 1,
                ProductId = 1,
                ItemQuantity = 2,
                UnitPrice = 100,
                ShippingAddress = "123 Test St",
                OrderDate = DateTime.Now,
                OrderStatus = "Shipped"
            };

            var product = new Product { ProductId = 1, ProductPrice = 100 };

            // Mocking the DbSet<Product> and setting up IQueryable for Product
            var productList = new List<Product> { product }.AsQueryable();
            var mockProductDbSet = new Mock<DbSet<Product>>();
            mockProductDbSet.As<IQueryable<Product>>()
                            .Setup(m => m.Provider)
                            .Returns(productList.Provider);
            mockProductDbSet.As<IQueryable<Product>>()
                            .Setup(m => m.Expression)
                            .Returns(productList.Expression);
            mockProductDbSet.As<IQueryable<Product>>()
                            .Setup(m => m.ElementType)
                            .Returns(productList.ElementType);
            mockProductDbSet.As<IQueryable<Product>>()
                            .Setup(m => m.GetEnumerator())
                            .Returns(productList.GetEnumerator());

            // Setting up the Products property of AppDbContext
            _dbContextMock.Setup(db => db.Products)
                          .Returns(mockProductDbSet.Object);

            // Mock the order service
            _orderServiceMock.Setup(service => service.UpdateOrderAsync(orderId, orderDto))
                             .ReturnsAsync(new Order { OrderId = orderId });

            // Mock the mapper
            _mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>()))
                       .Returns(orderDto);

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Debugging: Output result for inspection
            Console.WriteLine(result);  // Add a debug statement

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected an OkObjectResult but got null.");
            var returnedOrder = okResult?.Value as OrderDTO;
            Assert.IsNotNull(returnedOrder, "Returned OrderDTO was null.");
            Assert.AreEqual(orderDto.OrderId, returnedOrder?.OrderId);
        }


        [Test]
        public async Task UpdateOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = 1,
                ProductId = 1,
                ItemQuantity = 2,
                UnitPrice = 100,
                ShippingAddress = "123 Test St",
                OrderDate = DateTime.Now,
                OrderStatus = "Pending"
            };

            // Mocking the order service to return null for a non-existent order
            _orderServiceMock.Setup(service => service.UpdateOrderAsync(orderId, orderDto)).ReturnsAsync((Order)null);

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);  // Expecting NotFoundObjectResult
        }


        [Test]
        public async Task GetOrder_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDTO { OrderId = orderId, CustomerId = 1, ProductId = 1, ItemQuantity = 2, UnitPrice = 100, ShippingAddress = "123 Test St", OrderDate = DateTime.Now, OrderStatus = "Shipped" };

            _orderServiceMock.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync(new Order { OrderId = orderId });
            _mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>())).Returns(orderDto);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedOrder = okResult?.Value as OrderDTO;
            Assert.AreEqual(orderDto.OrderId, returnedOrder?.OrderId);
        }

        [Test]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;

            _orderServiceMock.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetOrder_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int orderId = 1;

            _orderServiceMock.Setup(service => service.GetOrderByIdAsync(orderId)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
        }
    }
}
