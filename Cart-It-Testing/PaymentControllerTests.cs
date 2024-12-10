using AutoMapper;
using Cart_It.Controllers;
using Cart_It.DTOs;
using Cart_It.Models;
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
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private Mock<IOrderService> _orderServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<PaymentController>> _loggerMock;
        private PaymentController _controller;

        [SetUp]
        public void SetUp()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _orderServiceMock = new Mock<IOrderService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PaymentController>>();
            _controller = new PaymentController(_paymentServiceMock.Object, _orderServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task AddPayment_ReturnsCreatedAtAction_WhenPaymentIsAdded()
        {
            // Arrange
            var paymentDto = new PaymentDTO
            {
                PaymentId = 1,
                OrderId = 1,
                CustomerId = 1,
                AmountToPay = 200,
                PaymentMethod = "CreditCard",
                PaymentStatus = "Completed",
                PaymentDate = DateTime.Now
            };

            var order = new Order { OrderId = 1, TotalAmount = 200 };

            // Mock the OrderService to return the order by OrderId
            _orderServiceMock.Setup(service => service.GetOrderByIdAsync(paymentDto.OrderId)).ReturnsAsync(order);

            // Mock the PaymentService to return the PaymentDTO after adding the payment
            _paymentServiceMock.Setup(service => service.AddPaymentAsync(It.IsAny<PaymentDTO>()))
                .ReturnsAsync(new PaymentDTO
                {
                    PaymentId = 1,
                    OrderId = 1,
                    AmountToPay = 200.00m
                });

            // Act: Call the AddPayment method in the controller
            var result = await _controller.AddPayment(paymentDto);

            // Assert: Verify that the result is not null
            Assert.IsNotNull(result);

            // Assert: Verify that the result is of type CreatedAtActionResult
            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            // Assert: Verify that the action name is correct (GetPayment)
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.AreEqual(nameof(PaymentController.GetPayment), createdAtActionResult?.ActionName);

            // Assert: Verify that the returned PaymentDTO has the correct PaymentId and AmountToPay
            var returnedPayment = createdAtActionResult?.Value as PaymentDTO;
            Assert.AreEqual(paymentDto.PaymentId, returnedPayment?.PaymentId);
            Assert.AreEqual(paymentDto.AmountToPay, returnedPayment?.AmountToPay);

            // Verify that the AddPaymentAsync method was called once with the correct parameter
            _paymentServiceMock.Verify(service => service.AddPaymentAsync(It.IsAny<PaymentDTO>()), Times.Once);
        }


        [Test]
        public async Task AddPayment_ReturnsBadRequest_WhenPaymentDtoIsNull()
        {
            // Act
            var result = await _controller.AddPayment(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdatePayment_ReturnsOk_WhenPaymentIsUpdated()
        {
            // Arrange
            var paymentId = 1;
            var paymentDto = new PaymentDTO
            {
                PaymentId = paymentId,
                OrderId = 1,
                CustomerId = 1,
                AmountToPay = 200,
                PaymentMethod = "CreditCard",
                PaymentStatus = "Completed",
                PaymentDate = DateTime.Now
            };

            var order = new Order { OrderId = 1, TotalAmount = 200 };

            // Mock the OrderService to return the order by OrderId
            _orderServiceMock.Setup(service => service.GetOrderByIdAsync(paymentDto.OrderId)).ReturnsAsync(order);

            // Mock the PaymentService to return the updated PaymentDTO after updating the payment
            _paymentServiceMock.Setup(service => service.UpdatePaymentAsync(paymentId, It.IsAny<PaymentDTO>()))
                .ReturnsAsync(new PaymentDTO
                {
                    PaymentId = paymentId,
                    OrderId = 1,
                    AmountToPay = 200.00m,
                    PaymentMethod = "CreditCard",
                    PaymentStatus = "Completed",
                    PaymentDate = DateTime.Now
                });

            // Act: Call the UpdatePayment method in the controller
            var result = await _controller.UpdatePayment(paymentId, paymentDto);

            // Assert: Verify that the result is of type OkObjectResult
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;

            // Assert: Verify that the returned PaymentDTO has the correct PaymentId
            var updatedPayment = okResult?.Value as PaymentDTO;
            Assert.AreEqual(paymentDto.PaymentId, updatedPayment?.PaymentId);
            Assert.AreEqual(paymentDto.AmountToPay, updatedPayment?.AmountToPay);
            Assert.AreEqual(paymentDto.PaymentMethod, updatedPayment?.PaymentMethod);
            Assert.AreEqual(paymentDto.PaymentStatus, updatedPayment?.PaymentStatus);

            // Verify that the UpdatePaymentAsync method was called once with the correct parameters
            _paymentServiceMock.Verify(service => service.UpdatePaymentAsync(paymentId, It.IsAny<PaymentDTO>()), Times.Once);
        }

        [Test]
        public async Task UpdatePayment_ReturnsBadRequest_WhenPaymentDtoIsNull()
        {
            // Act
            var result = await _controller.UpdatePayment(1, null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetPayment_ReturnsOk_WhenPaymentExists()
        {
            // Arrange
            var paymentId = 1;

            // Create the expected PaymentDTO that will be returned by the mock service
            var expectedPayment = new PaymentDTO { PaymentId = paymentId, OrderId = 1, AmountToPay = 100.00m };

            // Setup the service mock to return the expected PaymentDTO
            _paymentServiceMock.Setup(service => service.GetPaymentByIdAsync(paymentId))
                .ReturnsAsync(expectedPayment);

            // Act
            var result = await _controller.GetPayment(paymentId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var returnedPayment = okResult?.Value as PaymentDTO;

            // Assert that the returned PaymentDTO matches the expected one
            Assert.AreEqual(paymentId, returnedPayment?.PaymentId);
            Assert.AreEqual(expectedPayment.AmountToPay, returnedPayment?.AmountToPay);
            Assert.AreEqual(expectedPayment.OrderId, returnedPayment?.OrderId);
            Assert.AreEqual(1, returnedPayment?.OrderId);

        }


        [Test]
        public async Task GetPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = 1;
            _paymentServiceMock.Setup(service => service.GetPaymentByIdAsync(paymentId))
                .ReturnsAsync((PaymentDTO)null);

            // Act
            var result = await _controller.GetPayment(paymentId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
