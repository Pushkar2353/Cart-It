using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface IOrderService
    {
        Task<Order> AddOrderAsync(OrderDTO orderDto);
        Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Order> AddOrderAsync(OrderDTO orderDto)
        {
            // Map OrderDTO to Order
            var order = _mapper.Map<Order>(orderDto);

            // Ensure UnitPrice is set from ProductPrice
            var product = await _productRepository.GetProductByIdAsync(orderDto.ProductId); // Fetch product to ensure consistency
            if (product != null)
            {
                order.UnitPrice = product.ProductPrice; // Set UnitPrice to ProductPrice
            }

            return await _orderRepository.AddOrderAsync(order);
        }

        public async Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto)
        {
            // Update order using the repository
            return await _orderRepository.UpdateOrderAsync(orderId, orderDto);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }
    }
}
