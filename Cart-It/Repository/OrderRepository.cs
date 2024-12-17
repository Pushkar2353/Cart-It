using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface IOrderRepository
    {
        Task<Order> AddOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IEnumerable<OrderDTO>> GetOrdersBySellerIdAsync(int sellerId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();



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
            // Add and save order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }



        public async Task<Order> UpdateOrderAsync(int orderId, OrderDTO orderDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Order not found.");

            // Fetch the product and set UnitPrice to ProductPrice
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == orderDto.ProductId);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            // Set UnitPrice to ProductPrice
            order.UnitPrice = product.ProductPrice;

            // Map other fields from orderDto to order
            _mapper.Map(orderDto, order);

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == orderDto.CustomerId);
            if (!customerExists)
                throw new InvalidOperationException("Customer not found.");

            // Update and save order
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.Products)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                }).ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersBySellerIdAsync(int sellerId)
        {
            return await _context.Orders
                .Where(o => o.Products.Any(p => p.SellerId == sellerId))  // Check if any product in the order matches the sellerId
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    ProductId = o.ProductId,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate,
                })
                .ToListAsync();
        }
    }
}
