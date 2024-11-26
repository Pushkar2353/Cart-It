using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Mapping;
using Cart_It.Models.JWT;
using Cart_It.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cart_It.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                // Verify credentials in Admin table
                var admin = _context.Administrator.SingleOrDefault(a => a.Email == request.Email && a.Password == request.Password);
                if (admin != null)
                {
                    return GenerateToken(admin.Email, "Admin");
                }

                // Verify credentials in Customer table
                var customer = _context.Customers.SingleOrDefault(c => c.Email == request.Email && c.Password == request.Password);
                if (customer != null)
                {
                    return GenerateToken(customer.Email, "Customer");
                }

                // Verify credentials in Seller table
                var seller = _context.Sellers.SingleOrDefault(s => s.Email == request.Email && s.Password == request.Password);
                if (seller != null)
                {
                    return GenerateToken(seller.Email, "Seller");
                }

                _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                return Unauthorized(new { Message = "Invalid email or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for email: {Email}", request.Email);
                return StatusCode(500, new { Message = "An error occurred during login. Please try again later." });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Validate the refresh token request
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { Message = "Refresh token is required." });
                }

                // Validate the refresh token (This can be done by decoding the JWT and checking claims)
                var principal = ValidateJwtToken(request.RefreshToken);
                if (principal == null)
                {
                    return Unauthorized(new { Message = "Invalid or expired refresh token." });
                }

                // Extract user information from the token (email, role, etc.)
                var email = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var role = principal.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                // If no email or role, return unauthorized
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                {
                    return Unauthorized(new { Message = "Invalid refresh token payload." });
                }

                // Retrieve the corresponding user based on the role and email
                object user = null;

                if (role == "Admin")
                {
                    user = _context.Administrator.SingleOrDefault(a => a.Email == email);
                }
                else if (role == "Customer")
                {
                    user = _context.Customers.SingleOrDefault(c => c.Email == email);
                }
                else if (role == "Seller")
                {
                    user = _context.Sellers.SingleOrDefault(s => s.Email == email);
                }

                if (user == null)
                {
                    return Unauthorized(new { Message = "Invalid user." });
                }

                // Generate a new refresh token for the corresponding user (Admin, Customer, or Seller)
                var newRefreshToken = GenerateRefreshToken(email);  // Use email for generating the new refresh token

                // Save or store the new refresh token in-memory or return it
                SaveRefreshToken(newRefreshToken);

                // Return the new access and refresh tokens
                var accessToken = IssueAccessToken(user);  // Generate a new access token using the user information

                return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                // Log the error and return a 500 error if something goes wrong
                _logger.LogError(ex, "An error occurred while refreshing the token.");
                return StatusCode(500, new { Message = "An error occurred while refreshing the token. Please try again later." });
            }
        }

        // Generate JWT Token
        private IActionResult GenerateToken(string email, string role)
        {
            var secretKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("JWT Secret Key is not configured.");
                throw new InvalidOperationException("JWT Secret Key is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Role", role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("Token generated successfully for email: {Email}, Role: {Role}", email, role);

            return Ok(new
            {
                Token = tokenString,
                Role = role
            });
        }

        // Method to retrieve and verify user based on email (simulating your existing pattern for Admin, Customer, Seller)
        private object GetUserByEmail(string email)
        {
            // Verify credentials in Admin table
            var admin = _context.Administrator.SingleOrDefault(a => a.Email == email);
            if (admin != null)
            {
                return admin;  // Return the admin if found
            }

            // Verify credentials in Customer table
            var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
            if (customer != null)
            {
                return customer;  // Return the customer if found
            }

            // Verify credentials in Seller table
            var seller = _context.Sellers.SingleOrDefault(s => s.Email == email);
            if (seller != null)
            {
                return seller;  // Return the seller if found
            }

            return null; // Return null if no matching user found
        }

        // Method to validate the JWT token
        private ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                return tokenHandler.ValidateToken(token, parameters, out _);
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Refresh token expired.");
                throw; // or return null if you prefer
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token.");
                throw; // or return null if you prefer
            }
        }

        // Method to issue a new access token
        private string IssueAccessToken(dynamic user) // Use dynamic type here to support Customer, Admin, or Seller
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("Role", user.Role)  // Assuming each user type (Customer, Admin, Seller) has a 'Role' property
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),  // Access token expiration time
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Method to generate a new refresh token using the email
        private string GenerateRefreshToken(string email)
        {
            // Simple refresh token generation, can be improved to suit your needs
            return Guid.NewGuid().ToString();  // Generate a unique token for the email
        }

        public class RefreshTokenStore
        {
            // Static list to store refresh tokens (you can replace this with a database or another persistent store)
            public static List<RefreshToken> Tokens = new List<RefreshToken>();
        }

        // Method to save the refresh token
        private void SaveRefreshToken(string refreshToken)
        {
            // Create a new RefreshToken object and add it to the static list
            RefreshTokenStore.Tokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTime.UtcNow.AddDays(7)  // Set expiration (e.g., 7 days)
            });

            // You can also log the added token if needed for debugging
            _logger.LogInformation($"Refresh token saved: {refreshToken}");
        }
    }
}
