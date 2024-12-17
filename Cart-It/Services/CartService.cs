using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Services
{
    public interface ICartService
    {
        Task<CartDTO> GetCartByIdAsync(int cartId);
        Task<IEnumerable<CartDTO>> GetAllCartsAsync();
        Task<CartDTO> AddCartAsync(CartDTO cartDto);
        Task<CartDTO> UpdateCartAsync(int productId, CartDTO cartDto);
        Task DeleteCartAsync(int cartId);
        Task<IEnumerable<CartDTO>> GetCartsByCustomerIdAsync(int customerId);

    }

    public class CartService : ICartService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public CartService(ICartRepository cartRepository, AppDbContext context, IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartByIdAsync(int cartId)
        {
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<IEnumerable<CartDTO>> GetAllCartsAsync()
        {
            var carts = await _cartRepository.GetAllCartsAsync();
            return _mapper.Map<IEnumerable<CartDTO>>(carts);
        }

        public async Task<CartDTO> AddCartAsync(CartDTO cartDto)
        {
            // Validate input
            if (cartDto == null)
                throw new ArgumentNullException(nameof(cartDto), "Cart data is null.");

            // Map CartDTO to Cart entity
            var cart = _mapper.Map<Cart>(cartDto);

            // Fetch product and validate
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cart.ProductId);
            if (product == null)
                throw new InvalidOperationException("The specified product does not exist.");

            // Fetch customer and validate
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == cart.CustomerId);
            if (!customerExists)
                throw new InvalidOperationException("The specified customer does not exist.");

            // Set the amount based on ProductPrice
            cart.Amount = product.ProductPrice;

            // Save cart to the repository
            await _cartRepository.AddCartAsync(cart);

            // Map the saved entity back to CartDTO
            var createdCartDto = _mapper.Map<CartDTO>(cart);
            return createdCartDto;
        }


        public async Task<CartDTO> UpdateCartAsync(int productId, CartDTO cartDto)
        {
            // Fetch the product to get the ProductPrice
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return null; // Product not found
            }

            // Update the cart DTO with product price
            cartDto.Amount = product.ProductPrice;

            // Update the cart with the new details
            return await _cartRepository.UpdateCartAsync(cartDto);
        }


        public async Task DeleteCartAsync(int cartId)
        {
            await _cartRepository.DeleteCartAsync(cartId);
        }

        public async Task<IEnumerable<CartDTO>> GetCartsByCustomerIdAsync(int customerId)
        {
            var carts = await _context.Carts
                .Where(cart => cart.CustomerId == customerId)
                .ToListAsync();

            // Map to DTOs if needed
            return carts.Select(cart => new CartDTO
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                ProductId = cart.ProductId,
                CartQuantity = cart.CartQuantity,
                Amount = cart.Amount,
                CreatedDate = cart.CreatedDate,
                UpdatedDate = cart.UpdatedDate
            });
        }

    }

}
