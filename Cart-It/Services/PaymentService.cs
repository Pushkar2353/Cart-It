using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface IPaymentService
    {
        Task<Payment> AddPaymentAsync(PaymentDTO paymentDto);
        Task<Payment> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<Payment> AddPaymentAsync(PaymentDTO paymentDto)
        {
            var payment = _mapper.Map<Payment>(paymentDto); // Map DTO to entity
            return await _paymentRepository.AddPaymentAsync(payment);
        }

        public async Task<Payment> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto)
        {
            return await _paymentRepository.UpdatePaymentAsync(paymentId, paymentDto);
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _paymentRepository.GetPaymentByIdAsync(paymentId);
        }
    }
}
