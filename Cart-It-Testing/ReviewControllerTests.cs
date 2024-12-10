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
    public class ReviewControllerTests
    {
        private Mock<IReviewService> _mockReviewService;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<ReviewController>> _mockLogger;
        private ReviewController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockReviewService = new Mock<IReviewService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ReviewController>>();
            _controller = new ReviewController(_mockReviewService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AddReview_ShouldReturnCreatedReview_WhenReviewIsValid()
        {
            // Arrange
            var reviewDto = new ReviewDTO
            {
                CustomerId = 1,
                ProductId = 1,
                Rating = 5,
                ReviewText = "Great product!",
                ReviewDate = DateTime.Now
            };

            var review = new Review
            {
                ReviewId = 1,
                CustomerId = 1,
                ProductId = 1,
                Rating = 5,
                ReviewText = "Great product!",
                ReviewDate = DateTime.Now
            };

            // Mock the AddReviewAsync service method to return the created review
            _mockReviewService.Setup(s => s.AddReviewAsync(reviewDto)).ReturnsAsync(review);

            // Mock the mapper to convert Review to ReviewDTO
            _mockMapper.Setup(m => m.Map<ReviewDTO>(review)).Returns(reviewDto);

            // Act
            var result = await _controller.AddReview(reviewDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult); // Ensure the result is not null
            Assert.AreEqual(201, createdResult.StatusCode); // Verify the status code is 201 (Created)
            Assert.AreEqual(reviewDto, createdResult.Value); // Ensure the returned value matches the ReviewDTO
        }

        [Test]
        public async Task AddReview_ShouldReturnBadRequest_WhenReviewDataIsNull()
        {
            // Act
            var result = await _controller.AddReview(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult); // Check that the result is a BadRequestObjectResult
            Assert.AreEqual(400, badRequestResult.StatusCode); // Verify the status code is 400
        }

        [Test]
        public async Task UpdateReview_ShouldReturnUpdatedReview_WhenReviewExists()
        {
            // Arrange
            int reviewId = 1;
            var reviewDto = new ReviewDTO { ReviewId = reviewId, CustomerId = 1, ProductId = 1, Rating = 5, ReviewText = "Updated review", ReviewDate = DateTime.Now };
            var review = new Review { ReviewId = reviewId, CustomerId = 1, ProductId = 1, Rating = 5, ReviewText = "Updated review", ReviewDate = DateTime.Now };
            _mockReviewService.Setup(s => s.UpdateReviewAsync(reviewId, reviewDto)).ReturnsAsync(review);
            _mockMapper.Setup(m => m.Map<ReviewDTO>(review)).Returns(reviewDto);

            // Act
            var result = await _controller.UpdateReview(reviewId, reviewDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(reviewDto, okResult.Value);
        }

        [Test]
        public async Task UpdateReview_ShouldReturnNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            int reviewId = 999; // Non-existing review
            var reviewDto = new ReviewDTO
            {
                ReviewId = reviewId,
                CustomerId = 1,
                ProductId = 1,
                Rating = 5,
                ReviewText = "Updated review",
                ReviewDate = DateTime.Now
            };

            // Simulate that the review does not exist
            _mockReviewService.Setup(s => s.UpdateReviewAsync(reviewId, reviewDto)).ReturnsAsync((Review)null);

            // Act
            var result = await _controller.UpdateReview(reviewId, reviewDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult; // Expect NotFoundObjectResult
            Assert.IsNotNull(notFoundResult); // Ensure result is not null
            Assert.AreEqual(404, notFoundResult.StatusCode); // Verify status code is 404
        }

        [Test]
        public async Task GetReview_ShouldReturnReview_WhenReviewExists()
        {
            // Arrange
            int reviewId = 1;
            var reviewDto = new ReviewDTO { ReviewId = reviewId, CustomerId = 1, ProductId = 1, Rating = 5, ReviewText = "Great product!", ReviewDate = DateTime.Now };
            var review = new Review { ReviewId = reviewId, CustomerId = 1, ProductId = 1, Rating = 5, ReviewText = "Great product!", ReviewDate = DateTime.Now };
            _mockReviewService.Setup(s => s.GetReviewByIdAsync(reviewId)).ReturnsAsync(review);
            _mockMapper.Setup(m => m.Map<ReviewDTO>(review)).Returns(reviewDto);

            // Act
            var result = await _controller.GetReview(reviewId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(reviewDto, okResult.Value);
        }

        [Test]
        public async Task GetReview_ShouldReturnNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            int reviewId = 999; // Non-existing review
            _mockReviewService.Setup(s => s.GetReviewByIdAsync(reviewId)).ReturnsAsync((Review)null);

            // Act
            var result = await _controller.GetReview(reviewId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); // Check that the result is NotFoundObjectResult
            Assert.AreEqual(404, notFoundResult.StatusCode); // Verify the status code is 404
        }

        [Test]
        public async Task GetReviewsByProductId_ShouldReturnReviews_WhenReviewsExist()
        {
            // Arrange
            int productId = 1;
            var reviewList = new List<ReviewDTO>
        {
            new ReviewDTO { ReviewId = 1, CustomerId = 1, ProductId = productId, Rating = 5, ReviewText = "Great product!", ReviewDate = DateTime.Now },
            new ReviewDTO { ReviewId = 2, CustomerId = 2, ProductId = productId, Rating = 4, ReviewText = "Good product", ReviewDate = DateTime.Now }
        };
            var reviews = new List<Review>
        {
            new Review { ReviewId = 1, CustomerId = 1, ProductId = productId, Rating = 5, ReviewText = "Great product!", ReviewDate = DateTime.Now },
            new Review { ReviewId = 2, CustomerId = 2, ProductId = productId, Rating = 4, ReviewText = "Good product", ReviewDate = DateTime.Now }
        };
            _mockReviewService.Setup(s => s.GetReviewsByProductIdAsync(productId)).ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDTO>>(reviews)).Returns(reviewList);

            // Act
            var result = await _controller.GetReviewsByProductId(productId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(reviewList, okResult.Value);
        }

        [Test]
        public async Task GetReviewsByProductId_ShouldReturnNotFound_WhenNoReviewsExist()
        {
            // Arrange
            int productId = 999; // Non-existing product
            _mockReviewService.Setup(s => s.GetReviewsByProductIdAsync(productId)).ReturnsAsync(new List<Review>()); // No reviews for the product

            // Act
            var result = await _controller.GetReviewsByProductId(productId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult; // Expect NotFoundObjectResult
            Assert.IsNotNull(notFoundResult); // Ensure result is not null
            Assert.AreEqual(404, notFoundResult.StatusCode); // Verify status code is 404
        }
    }
}
