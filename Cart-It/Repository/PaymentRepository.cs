﻿using AutoMapper;
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

            var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == payment.OrderId);
            if (!orderExists)
            {
                throw new InvalidOperationException("Order not found.");
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

            // Mapping values from DTO to entity, updating only changed fields
            _mapper.Map(paymentDto, payment);

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                                 .Include(p => p.Orders)
                                 .Include(p => p.Customers)
                                 .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }
    }
}