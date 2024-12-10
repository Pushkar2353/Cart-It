using AutoMapper;
using Cart_It.Controllers;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace Cart_It_Testing
{
    [TestFixture]
    public class CustomerControllerTests
    {
        private Mock<ICustomerService> _customerServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<CustomerController>> _loggerMock;
        private CustomerController _controller;

        [SetUp]
        public void Setup()
        {
            // Initialize the mocks
            _customerServiceMock = new Mock<ICustomerService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CustomerController>>();

            // Instantiate the controller with mocked dependencies
            _controller = new CustomerController(_mapperMock.Object, _customerServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetCustomerById_CustomerExists_ReturnsOkResult()
        {
            // Arrange
            var customerId = 1;
            var customer = new CustomerDTO { CustomerId = customerId, FirstName = "John", LastName = "Doe" };
            _customerServiceMock.Setup(service => service.GetCustomerByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            var result = await _controller.GetCustomerById(customerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetCustomerById_CustomerNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var customerId = 1;
            _customerServiceMock.Setup(service => service.GetCustomerByIdAsync(customerId)).ReturnsAsync((CustomerDTO)null);

            // Act
            var result = await _controller.GetCustomerById(customerId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetAllCustomers_ReturnsOkResult()
        {
            // Arrange
            var customers = new List<CustomerDTO>
    {
        new CustomerDTO { CustomerId = 1, FirstName = "John", LastName = "Doe" },
        new CustomerDTO { CustomerId = 2, FirstName = "Jane", LastName = "Doe" }
    };
            _customerServiceMock.Setup(service => service.GetAllCustomersAsync()).ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAllCustomers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            // Use JObject to parse and inspect the response structure
            var response = JObject.FromObject(okResult.Value);

            Assert.IsNotNull(response["Message"]);
            Assert.AreEqual("All customers fetched successfully.", response["Message"].ToString());
            Assert.IsNotNull(response["Customers"]);
            Assert.AreEqual(customers.Count, response["Customers"].Count());
        }

        [Test]
        public async Task CreateCustomer_ValidCustomer_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var customerDto = new CustomerDTO { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            var createdCustomer = new CustomerDTO { CustomerId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _customerServiceMock.Setup(service => service.AddCustomerAsync(customerDto)).ReturnsAsync(createdCustomer);

            // Act
            var result = await _controller.CreateCustomer(customerDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task UpdateCustomerPartial_CustomerExists_ReturnsOkResult()
        {
            // Arrange
            var customerId = 1;
            var customerDto = new CustomerDTO { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _customerServiceMock.Setup(service => service.UpdateCustomerPartialAsync(customerId, customerDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCustomerPartial(customerId, customerDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task UpdateCustomerPartial_CustomerNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var customerId = 1;
            var customerDto = new CustomerDTO { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _customerServiceMock.Setup(service => service.UpdateCustomerPartialAsync(customerId, customerDto)).ThrowsAsync(new KeyNotFoundException("Customer not found"));

            // Act
            var result = await _controller.UpdateCustomerPartial(customerId, customerDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task DeleteCustomer_CustomerExists_ReturnsOkResult()
        {
            // Arrange
            var customerId = 1;
            _customerServiceMock.Setup(service => service.DeleteCustomerAsync(customerId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task DeleteCustomer_CustomerNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var customerId = 1;
            _customerServiceMock.Setup(service => service.DeleteCustomerAsync(customerId)).ThrowsAsync(new KeyNotFoundException("Customer not found"));

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}