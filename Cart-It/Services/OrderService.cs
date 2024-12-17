using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Services
{
    public interface IOrderService
    {
        Task<Order> AddOrderAsync(OrderDTO orderDto);
        Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();


    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, AppDbContext context,IMapper mapper)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<Order> AddOrderAsync(OrderDTO orderDto)
        {
            // Map OrderDTO to Order
            var order = _mapper.Map<Order>(orderDto);

            // Fetch the product and ensure consistency
            var product = await _productRepository.GetProductByIdAsync(orderDto.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            // Set UnitPrice to ProductPrice
            order.UnitPrice = product.ProductPrice;

            // Ensure the customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == orderDto.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            // Delegate to the repository to save the order
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

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

    }
}
