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
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Order> AddOrderAsync(OrderDTO orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            return await _orderRepository.AddOrderAsync(order);
        }

        public async Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto)
        {
            return await _orderRepository.UpdateOrderAsync(orderId, orderDto);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }
    }
}
