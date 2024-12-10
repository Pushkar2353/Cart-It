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
    public class SellerControllerTests
    {
        private Mock<ISellerService> _mockSellerService;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<SellerController>> _mockLogger;
        private SellerController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockSellerService = new Mock<ISellerService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<SellerController>>();
            _controller = new SellerController(_mockSellerService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetSellerById_ShouldReturnSeller_WhenSellerExists()
        {
            // Arrange
            int sellerId = 1;
            var sellerDto = new SellerDTO { SellerId = sellerId, FirstName = "John", LastName = "Doe" };
            _mockSellerService.Setup(s => s.GetSellerByIdAsync(sellerId)).ReturnsAsync(sellerDto);

            // Act
            var result = await _controller.GetSellerById(sellerId);

            // Assert
            var actionResult = result as ActionResult<SellerDTO>;
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(sellerDto, okResult.Value);
        }

        [Test]
        public async Task GetSellerById_ShouldReturnNotFound_WhenSellerDoesNotExist()
        {
            // Arrange
            int sellerId = 999; // Non-existing ID
            _mockSellerService.Setup(s => s.GetSellerByIdAsync(sellerId)).ReturnsAsync((SellerDTO)null);

            // Act
            var result = await _controller.GetSellerById(sellerId);

            // Assert
            var actionResult = result as ActionResult<SellerDTO>;
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetAllSellers_ShouldReturnAllSellers()
        {
            // Arrange
            var sellerList = new List<SellerDTO>
        {
            new SellerDTO { SellerId = 1, FirstName = "John", LastName = "Doe" },
            new SellerDTO { SellerId = 2, FirstName = "Jane", LastName = "Smith" }
        };
            _mockSellerService.Setup(s => s.GetAllSellersAsync()).ReturnsAsync(sellerList);

            // Act
            var result = await _controller.GetAllSellers();

            // Assert
            var actionResult = result as ActionResult<IEnumerable<SellerDTO>>;
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(sellerList, okResult.Value);
        }

        [Test]
        public async Task CreateSeller_ShouldReturnCreatedSeller()
        {
            // Arrange
            var sellerDto = new SellerDTO
            {
                FirstName = "New",
                LastName = "Seller",
                Email = "new.seller@example.com",
                Password = "Password123!",
                SellerPhoneNumber = "1234567890",
                Gender = "Male",
                CompanyName = "New Seller Inc.",
                AddressLine1 = "123 Main St",
                City = "City",
                State = "State",
                Country = "Country",
                PinCode = "123456",
                GSTIN = "12ABCDE3456Z1",
                BankAccountNumber = "123456789012345"
            };
            var createdSeller = new SellerDTO { SellerId = 1, FirstName = "New", LastName = "Seller" };
            _mockSellerService.Setup(s => s.CreateSellerAsync(sellerDto)).ReturnsAsync(createdSeller);

            // Act
            var result = await _controller.CreateSeller(sellerDto);

            // Assert
            var actionResult = result as ActionResult<SellerDTO>;
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(createdSeller, createdResult.Value);
        }

        [Test]
        public async Task UpdateSeller_ShouldReturnUpdatedSeller_WhenSellerExists()
        {
            // Arrange
            int sellerId = 1;
            var sellerDto = new SellerDTO { SellerId = sellerId, FirstName = "Updated", LastName = "Seller" };
            _mockSellerService.Setup(s => s.UpdateSellerAsync(sellerId, sellerDto)).ReturnsAsync(sellerDto);

            // Act
            var result = await _controller.UpdateSeller(sellerId, sellerDto);

            // Assert
            var actionResult = result as ActionResult<SellerDTO>;
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(sellerDto, okResult.Value);
        }

        [Test]
        public async Task UpdateSeller_ShouldReturnNotFound_WhenSellerDoesNotExist()
        {
            // Arrange
            int sellerId = 999; // Non-existing seller
            var sellerDto = new SellerDTO { SellerId = sellerId, FirstName = "Updated", LastName = "Seller" };
            _mockSellerService.Setup(s => s.UpdateSellerAsync(sellerId, sellerDto)).ReturnsAsync((SellerDTO)null);

            // Act
            var result = await _controller.UpdateSeller(sellerId, sellerDto);

            // Assert
            var actionResult = result as ActionResult<SellerDTO>;
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task DeleteSeller_ShouldReturnNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            int sellerId = 1;
            _mockSellerService.Setup(s => s.DeleteSellerAsync(sellerId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteSeller(sellerId);

            // Assert
            var actionResult = result as ActionResult;
            Assert.IsInstanceOf<NoContentResult>(actionResult);
        }

        [Test]
        public async Task DeleteSeller_ShouldReturnNotFound_WhenSellerDoesNotExist()
        {
            // Arrange
            int sellerId = 999; // Non-existing seller
            _mockSellerService.Setup(s => s.DeleteSellerAsync(sellerId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteSeller(sellerId);

            // Assert
            var actionResult = result as ActionResult;
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
