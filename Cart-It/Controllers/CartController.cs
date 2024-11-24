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
        public CartController(ICartService cartService, AppDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCartsAsync();
            return Ok(carts);
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
            {
                return BadRequest("Cart data is null.");
            }

            // Check if Customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == cartDto.CustomerId);
            if (!customerExists)
            {
                return BadRequest("Customer does not exist.");
            }

            // Check if Product exists
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == cartDto.ProductId);
            if (!productExists)
            {
                return BadRequest("Product does not exist.");
            }

            await _cartService.AddCartAsync(cartDto);
            return CreatedAtAction(nameof(GetCartById), new { id = cartDto.CartId }, cartDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] CartDTO cartDto)
        {
            if (cartDto == null || id != cartDto.CartId)
            {
                return BadRequest();
            }

            await _cartService.UpdateCartAsync(id, cartDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            await _cartService.DeleteCartAsync(id);
            return NoContent();
        }
    }

}
