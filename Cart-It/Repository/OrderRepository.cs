using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface IOrderRepository
    {
        Task<Order> AddOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == order.ProductId);
            if (!productExists)
            {
                throw new InvalidOperationException("Product not found.");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == order.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // Only update the changed fields
            _mapper.Map(orderDto, order);

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == order.ProductId);
            if (!productExists)
            {
                throw new InvalidOperationException("Product not found.");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == order.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                                 .Include(o => o.Products)
                                 .Include(o => o.Customers)
                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
