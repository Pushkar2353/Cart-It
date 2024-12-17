using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByIdAsync(int cartId);
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task AddCartAsync(Cart cart);
        Task<CartDTO> UpdateCartAsync(CartDTO cartDto);
        Task DeleteCartAsync(int cartId);
    }

    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Customers)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            return await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Customers)
                .ToListAsync();
        }

        public async Task AddCartAsync(Cart cart)
        {
            // Ensure cart is valid
            if (cart == null)
                throw new ArgumentNullException(nameof(cart), "Cart entity is null.");

            // Add cart to database
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }


        public async Task<CartDTO> UpdateCartAsync(CartDTO cartDto)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartDto.CartId);
            if (cart == null)
            {
                return null; // Cart not found
            }

            // Update cart details
            cart.Amount = cartDto.Amount;
            cart.CartQuantity = cartDto.CartQuantity;

            // Update cart in the database
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            return cartDto; // Return updated cart DTO
        }

        public async Task DeleteCartAsync(int cartId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }

}
