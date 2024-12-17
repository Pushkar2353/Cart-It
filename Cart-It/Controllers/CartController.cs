using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly AppDbContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, AppDbContext context, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            _logger.LogInformation("Fetching cart with ID {CartId}.", id);

            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                _logger.LogWarning("Cart with ID {CartId} not found.", id);
                return NotFound(new { message = "Cart not found." });
            }

            _logger.LogInformation("Successfully fetched cart with ID {CartId}.", id);
            return Ok(cart);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            _logger.LogInformation("Fetching all carts.");

            var carts = await _cartService.GetAllCartsAsync();

            _logger.LogInformation("Successfully fetched {Count} carts.", carts.Count());
            return Ok(carts);
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartDTO cartDto)
        {
            try
            {
                // Validate request body
                if (cartDto == null)
                    return BadRequest(new { message = "Cart data is required." });

                // Call service to add cart
                var createdCart = await _cartService.AddCartAsync(cartDto);

                // Return 201 Created response
                return CreatedAtAction(nameof(AddCart), new { id = createdCart.CartId }, createdCart);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCart(int productId, [FromBody] CartDTO cartDto)
        {
            if (cartDto == null || productId != cartDto.ProductId)
            {
                _logger.LogWarning("Invalid data for updating cart with ProductId {ProductId}.", productId);
                return BadRequest(new { message = "Invalid cart data." });
            }

            try
            {
                // Proceed with updating the cart using product ID
                var result = await _cartService.UpdateCartAsync(productId, cartDto);
                if (result == null)
                {
                    return NotFound(new { message = "Cart not found for the given product." });
                }

                // Return NoContentResult (204 No Content) after successful update
                return NoContent(); // This ensures a 204 status code
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart with ProductId {ProductId}.", productId);
                return StatusCode(500, new { message = "An error occurred while updating the cart." });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            try
            {
                _logger.LogInformation("Deleting cart with ID {CartId}.", id);
                await _cartService.DeleteCartAsync(id);

                _logger.LogInformation("Successfully deleted cart with ID {CartId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting cart with ID {CartId}.", id);
                return StatusCode(500, new { message = "An error occurred while deleting the cart." });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCartsByCustomerId(int customerId)
        {
            _logger.LogInformation("Fetching carts for customer ID {CustomerId}.", customerId);

            try
            {
                // Check if the customer exists
                var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
                if (!customerExists)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} does not exist.", customerId);
                    return NotFound(new { message = "Customer not found." });
                }

                // Fetch carts for the customer
                var carts = await _cartService.GetCartsByCustomerIdAsync(customerId);

                if (carts == null || !carts.Any())
                {
                    _logger.LogWarning("No carts found for customer ID {CustomerId}.", customerId);
                    return NotFound(new { message = "No carts found for this customer." });
                }

                _logger.LogInformation("Successfully fetched {Count} carts for customer ID {CustomerId}.", carts.Count(), customerId);
                return Ok(carts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching carts for customer ID {CustomerId}.", customerId);
                return StatusCode(500, new { message = "An error occurred while fetching carts." });
            }
        }

    }
}
