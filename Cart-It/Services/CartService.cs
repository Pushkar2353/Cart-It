using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface ICartService
    {
        Task<CartDTO> GetCartByIdAsync(int cartId);
        Task<IEnumerable<CartDTO>> GetAllCartsAsync();
        Task AddCartAsync(CartDTO cartDto);
        Task UpdateCartAsync(int cartId, CartDTO cartDto);
        Task DeleteCartAsync(int cartId);
    }

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
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

        public async Task AddCartAsync(CartDTO cartDto)
        {
            try
            {
                var cart = _mapper.Map<Cart>(cartDto);
                await _cartRepository.AddCartAsync(cart);
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
            await _cartRepository.UpdateCartAsync(cartId, cart);
        }

        public async Task DeleteCartAsync(int cartId)
        {
            await _cartRepository.DeleteCartAsync(cartId);
        }
    }

}
