using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (cartDto == null)
            {
                _logger.LogWarning("Received null cart data.");
                return BadRequest(new { message = "Cart data is null." });
            }

            try
            {
                _logger.LogInformation("Checking if customer with ID {CustomerId} exists.", cartDto.CustomerId);
                var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == cartDto.CustomerId);
                if (!customerExists)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} does not exist.", cartDto.CustomerId);
                    return BadRequest(new { message = "Customer does not exist." });
                }

                _logger.LogInformation("Checking if product with ID {ProductId} exists.", cartDto.ProductId);
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == cartDto.ProductId);
                if (!productExists)
                {
                    _logger.LogWarning("Product with ID {ProductId} does not exist.", cartDto.ProductId);
                    return BadRequest(new { message = "Product does not exist." });
                }

                // Fetch product to get the ProductPrice
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartDto.ProductId);
                if (product != null)
                {
                    cartDto.Amount = product.ProductPrice;  // Set Amount to the ProductPrice
                }

                _logger.LogInformation("Adding cart for customer ID {CustomerId} and product ID {ProductId}.", cartDto.CustomerId, cartDto.ProductId);
                var cart = await _cartService.AddCartAsync(cartDto);

                _logger.LogInformation("Successfully added cart with ID {CartId}.", cartDto.CartId);
                return CreatedAtAction(nameof(GetCartById),new { cartId = cartDto.CartId },cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding cart.");
                return StatusCode(500, new { message = "An error occurred while adding the cart." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] CartDTO cartDto)
        {
            if (cartDto == null || id != cartDto.CartId)
            {
                _logger.LogWarning("Invalid data for updating cart with ID {CartId}.", id);
                return BadRequest(new { message = "Invalid cart data." });
            }

            try
            {
                // Fetch product to get the ProductPrice
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartDto.ProductId);
                if (product != null)
                {
                    cartDto.Amount = product.ProductPrice;  // Set Amount to the ProductPrice
                }

                // Proceed with updating the cart
                await _cartService.UpdateCartAsync(id, cartDto);

                // Return NoContentResult (204 No Content) after successful update
                return NoContent(); // This ensures a 204 status code
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart with ID {CartId}.", id);
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
    }
}
