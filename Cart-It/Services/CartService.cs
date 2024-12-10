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
        Task UpdateCartAsync(int cartId, CartDTO cartDto);
        Task DeleteCartAsync(int cartId);
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
            try
            {
                // Map CartDTO to Cart entity
                var cart = _mapper.Map<Cart>(cartDto);

                // Fetch the product and set the Amount to the ProductPrice
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cart.ProductId);
                if (product != null)
                {
                    cart.Amount = product.ProductPrice;
                }

                // Add the cart to the repository
                await _cartRepository.AddCartAsync(cart);

                // After saving the cart, return the cartDTO with CartId and Amount
                var createdCartDto = _mapper.Map<CartDTO>(cart);  // Map the saved cart entity back to CartDTO
                return createdCartDto;  // Return the CartDTO with the CartId
            }
            catch (InvalidOperationException ex)
            {
                // Log or handle the error properly
                throw new Exception($"Error adding cart: {ex.Message}");
            }
        }


        public async Task UpdateCartAsync(int cartId, CartDTO cartDto)
        {
            var cart = _mapper.Map<Cart>(cartDto);

            // Fetch the product and set the Amount to the ProductPrice
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cart.ProductId);
            if (product != null)
            {
                cart.Amount = product.ProductPrice;
            }

            await _cartRepository.UpdateCartAsync(cartId, cart);
        }


        public async Task DeleteCartAsync(int cartId)
        {
            await _cartRepository.DeleteCartAsync(cartId);
        }
    }

}
