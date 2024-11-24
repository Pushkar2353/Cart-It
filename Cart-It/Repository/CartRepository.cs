using Cart_It.Data;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByIdAsync(int cartId);
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(int cartId, Cart cart);
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
            // Check if CustomerId exists in the Customers table
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == cart.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("The specified customer does not exist.");
            }

            // Check if ProductId exists in the Products table
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == cart.ProductId);
            if (!productExists)
            {
                throw new InvalidOperationException("The specified product does not exist.");
            }

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateCartAsync(int cartId, Cart cart)
        {
            var existingCart = await GetCartByIdAsync(cartId);
            if (existingCart != null)
            {
                existingCart.CustomerId = cart.CustomerId != 0 ? cart.CustomerId : existingCart.CustomerId;
                existingCart.ProductId = cart.ProductId != 0 ? cart.ProductId : existingCart.ProductId;
                existingCart.CartQuantity = cart.CartQuantity != 0 ? cart.CartQuantity : existingCart.CartQuantity;
                existingCart.Amount = cart.Amount.HasValue ? cart.Amount : existingCart.Amount;
                existingCart.UpdatedDate = DateTime.Now;

                _context.Carts.Update(existingCart);
                await _context.SaveChangesAsync();
            }
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
