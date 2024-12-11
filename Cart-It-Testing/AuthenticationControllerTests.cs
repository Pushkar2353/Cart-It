using Cart_It.Controllers;
using Cart_It.Data;
using Cart_It.Models;
using Cart_It.Models.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cart_It_Testing
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<AppDbContext> _dbContextMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILogger<AuthenticationController>> _loggerMock;
        private AuthenticationController _controller;

        [SetUp]
        public void SetUp()
        {
            _dbContextMock = new Mock<AppDbContext>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthenticationController>>();
            _controller = new AuthenticationController(_dbContextMock.Object, _configurationMock.Object, _loggerMock.Object);
        }


        [Test]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "Password123" };

            // Create a mock administrator for the test
            var admin = new Administrator { Email = "test@example.com", Password = "Password123", AdminId = 1 };

            // Mock the DbSet for Administrator to return a list containing the mock admin
            var queryable = new List<Administrator> { admin }.AsQueryable();
            var dbSetMock = new Mock<DbSet<Administrator>>();
            dbSetMock.As<IQueryable<Administrator>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSetMock.As<IQueryable<Administrator>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<Administrator>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<Administrator>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Mock the DbContext to return the mocked DbSet for Administrator
            _dbContextMock.Setup(db => db.Administrator).Returns(dbSetMock.Object);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            if (objectResult.StatusCode == 500)
            {
                Console.WriteLine($"Error message: {objectResult.Value}");
                Assert.Fail($"Expected status code 200, but got 500. Error: {objectResult.Value}");
            }
            else
            {
                Assert.AreEqual(200, objectResult.StatusCode);

                // Additional check for the returned token
                var returnedValue = objectResult.Value as dynamic;
                Assert.IsNotNull(returnedValue);
                Assert.IsNotNull(returnedValue.token);
            }
        }

        [Test]
        public Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "invalid@example.com", Password = "WrongPassword" };

            var administrators = new List<Administrator>
            {
                // No administrators here, so it will return null for any search
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Administrator>>();
            mockSet.As<IQueryable<Administrator>>().Setup(m => m.Provider).Returns(administrators.Provider);
            mockSet.As<IQueryable<Administrator>>().Setup(m => m.Expression).Returns(administrators.Expression);
            mockSet.As<IQueryable<Administrator>>().Setup(m => m.ElementType).Returns(administrators.ElementType);
            mockSet.As<IQueryable<Administrator>>().Setup(m => m.GetEnumerator()).Returns(administrators.GetEnumerator());

            _dbContextMock.Setup(db => db.Administrator).Returns(mockSet.Object);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            if (result is ObjectResult objectResult)
            {
                Assert.IsNotNull(objectResult);
                Assert.AreEqual(401, objectResult.StatusCode); // Verify the 401 status code for unauthorized
                Assert.AreEqual("Invalid email or password.", objectResult?.Value?.ToString());
            }
            else
            {
                Assert.Fail("Expected ObjectResult, but got: " + result.GetType());
            }

            return Task.CompletedTask;
        }


        [Test]
        public Task Login_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "Password123" };

            // Simulate an exception when accessing the Administrator property
            _dbContextMock.Setup(db => db.Administrator)
                          .Throws(new Exception("Database error"));

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            if (result is ObjectResult objectResult)
            {
                Assert.AreEqual(500, objectResult?.StatusCode);

                // Check the actual value (it may be wrapped inside an object)
                var errorMessage = objectResult?.Value?.ToString();

                // You may need to adjust the error message check if the value is not exactly the string but a wrapped object
                StringAssert.Contains("An error occurred during login", errorMessage);  // This matches any string with that prefix
            }
            else
            {
                Assert.Fail("Expected ObjectResult, but got: " + result.GetType());
            }

            return Task.CompletedTask;
        }

        /*
         
        [Test]
        public async Task RefreshToken_ReturnsOk_WhenTokenIsValid()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, "test@example.com"),
        new Claim("Role", "Customer")
    };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

            // Simulate the ValidateJwtToken call
            _controller.GetType().GetMethod("ValidateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_controller, new object[] { refreshTokenRequest.RefreshToken });

            // Setup the mock to return a customer
            _dbContextMock.Setup(db => db.Customers.SingleOrDefault(It.IsAny<Func<Customer, bool>>()))
                .Returns(new Customer { Email = "test@example.com", CustomerId = 1 });

            // Setup the JWT configuration
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("some-secret-key");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest); // Await the result

            // Assert
            if (result is OkObjectResult okResult)
            {
                Assert.IsNotNull(okResult);
                Assert.That(okResult.Value, Is.InstanceOf(typeof(object)));
            }
            else
            {
                Assert.Fail("Expected OkObjectResult, but got: " + result.GetType());
            }
        }


        [Test]
        public async Task RefreshToken_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };

            // Simulate the ValidateJwtToken call
            _controller.GetType().GetMethod("ValidateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_controller, new object[] { refreshTokenRequest.RefreshToken });

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest); // Await the result

            // Assert
            if (result is UnauthorizedObjectResult unauthorizedResult)
            {
                Assert.IsNotNull(unauthorizedResult);
                Assert.AreEqual("Invalid or expired refresh token.", unauthorizedResult?.Value?.ToString());
            }
            else
            {
                Assert.Fail("Expected UnauthorizedObjectResult, but got: " + result.GetType());
            }
        }

        [Test]
        public async Task RefreshToken_ReturnsBadRequest_WhenRefreshTokenIsMissing()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { RefreshToken = null };

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest); // Await the result

            // Assert
            if (result is BadRequestObjectResult badRequestResult)
            {
                Assert.IsNotNull(badRequestResult);

                // Log or inspect the type of the result
                var errorValue = badRequestResult.Value;
                Assert.IsNotNull(errorValue, "The response does not contain a valid error message.");

                // Debugging: Log the type of errorValue
                Console.WriteLine($"Error value type: {errorValue.GetType()}");

                // Try to cast and access the message
                var errorMessage = errorValue as dynamic;
                if (errorMessage != null)
                {
                    // Ensure the message is the expected one
                    Assert.AreEqual("Refresh token is required.", errorMessage?.Message);
                }
                else
                {
                    // If errorMessage is null or not dynamic, inspect the value directly
                    Assert.AreEqual("Refresh token is required.", errorValue.ToString());
                }
            }
            else
            {
                Assert.Fail("Expected BadRequestObjectResult, but got: " + result.GetType());
            }
        }



        [Test]
        public async Task RefreshToken_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };

            // Simulate an exception
            _controller.GetType().GetMethod("ValidateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_controller, new object[] { refreshTokenRequest.RefreshToken });

            _dbContextMock.Setup(db => db.Customers.SingleOrDefault(It.IsAny<Func<Customer, bool>>()))
                .Throws(new Exception("Database error"));

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest); // Await the result

            // Assert
            if (result is ObjectResult objectResult)
            {
                Assert.AreEqual(500, objectResult?.StatusCode);
                Assert.AreEqual("An error occurred while refreshing the token. Please try again later.", objectResult?.Value?.ToString());
            }
            else
            {
                Assert.Fail("Expected ObjectResult, but got: " + result.GetType());
            }
        }
        */

    }
}