using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface IPaymentService
    {
        Task<PaymentDTO> AddPaymentAsync(PaymentDTO paymentDto);
        Task<PaymentDTO> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto);
        Task<PaymentDTO?> GetPaymentByIdAsync(int paymentId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PaymentDTO> AddPaymentAsync(PaymentDTO paymentDto)
        {
            // Retrieve the order to validate and synchronize AmountToPay
            var order = await _orderRepository.GetOrderByIdAsync(paymentDto.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // If AmountToPay is not provided in the DTO, use the TotalAmount from the Order
            if (!paymentDto.AmountToPay.HasValue)
            {
                paymentDto.AmountToPay = order.TotalAmount;
            }

            // If AmountToPay is still null after synchronization, throw an exception
            if (paymentDto.AmountToPay == null)
            {
                throw new InvalidOperationException("Amount To Pay cannot be null.");
            }

            // Map DTO to domain model (Payment)
            var payment = _mapper.Map<Payment>(paymentDto);

            // Add payment using the repository
            var addedPayment = await _paymentRepository.AddPaymentAsync(payment);

            // Return the payment DTO after adding it
            return _mapper.Map<PaymentDTO>(addedPayment);
        }

        public async Task<PaymentDTO> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto)
        {
            // Retrieve the order to validate and synchronize AmountToPay
            var order = await _orderRepository.GetOrderByIdAsync(paymentDto.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // If AmountToPay is not provided in the DTO, use the TotalAmount from the Order
            if (!paymentDto.AmountToPay.HasValue)
            {
                paymentDto.AmountToPay = order.TotalAmount;
            }

            // If AmountToPay is still null after synchronization, throw an exception
            if (paymentDto.AmountToPay == null)
            {
                throw new InvalidOperationException("Amount To Pay cannot be null.");
            }

            // Update payment using the repository
            var updatedPayment = await _paymentRepository.UpdatePaymentAsync(paymentId, paymentDto);

            // Return the updated payment DTO
            return _mapper.Map<PaymentDTO>(updatedPayment);
        }

        public async Task<PaymentDTO?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return null;
            }

            return _mapper.Map<PaymentDTO>(payment);
        }
    }
}
