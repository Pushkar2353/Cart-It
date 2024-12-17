using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<PaymentDTO>> GetPaymentsByCustomerIdAsync(int customerId);
        Task<IEnumerable<PaymentDTO>> GetPaymentsBySellerIdAsync(int sellerId);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();


    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PaymentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == payment.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == payment.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // Synchronize AmountToPay with Order's TotalAmount, handle null values safely
            if (!payment.AmountToPay.HasValue)
            {
                payment.AmountToPay = order.TotalAmount;
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == paymentDto.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // Synchronize AmountToPay with Order's TotalAmount, handle null values safely
            if (!paymentDto.AmountToPay.HasValue)
            {
                paymentDto.AmountToPay = order.TotalAmount;
            }

            // Map DTO to domain model (Payment)
            _mapper.Map(paymentDto, payment);

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                                 .Include(p => p.Orders) // Ensure Order navigation property is included
                                 .Include(p => p.Customers) // Ensure Customer navigation property is included
                                 .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Customers)
                .Include(p => p.Orders)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsByCustomerIdAsync(int customerId)
        {
            return await _context.Payments
                .Where(p => p.CustomerId == customerId)
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId,
                    PaymentDate = p.PaymentDate,
                    AmountToPay = p.AmountToPay
                }).ToListAsync();
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsBySellerIdAsync(int sellerId)
        {
            return await _context.Payments
                .Where(p => p.Orders.Products.Any(prod => prod.SellerId == sellerId))  // Check if any product in the order matches the sellerId
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId,
                    OrderId = p.OrderId,  // Assuming Payment has an OrderId property to reference the associated order
                    PaymentDate = p.PaymentDate,
                })
                .ToListAsync();
        }
    }

}
