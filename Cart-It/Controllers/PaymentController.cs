using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentController> _logger;
        private readonly IOrderService _orderService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService, IMapper mapper, ILogger<PaymentController> logger)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDto)
        {
            if (paymentDto == null)
            {
                _logger.LogWarning("AddPayment: Received null payment data.");
                return BadRequest("Payment data is null.");
            }

            try
            {
                // Validate if Order exists
                var order = await _orderService.GetOrderByIdAsync(paymentDto.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("AddPayment: Order with ID {OrderId} not found.", paymentDto.OrderId);
                    return NotFound($"Order with ID {paymentDto.OrderId} does not exist.");
                }

                // Set AmountToPay directly from TotalAmount of the order
                paymentDto.AmountToPay = order.TotalAmount;

                // Validate AmountToPay consistency
                if (paymentDto.AmountToPay != order.TotalAmount)
                {
                    _logger.LogWarning("AddPayment: AmountToPay mismatch. Expected {ExpectedAmount}, but received {ReceivedAmount}.", order.TotalAmount, paymentDto.AmountToPay);
                    return BadRequest($"AmountToPay mismatch. Expected: {order.TotalAmount}");
                }

                _logger.LogInformation("AddPayment: Adding a new payment.");
                var payment = await _paymentService.AddPaymentAsync(paymentDto);
                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response

                _logger.LogInformation("AddPayment: Successfully added payment with ID {PaymentId}.", payment.PaymentId);
                return CreatedAtAction(nameof(GetPayment), new { paymentId = payment.PaymentId }, payment);
            }
            catch (Exception ex)
            {
                // Log full exception details to help diagnose the issue
                _logger.LogError(ex, "AddPayment: An error occurred while adding the payment.");
                return StatusCode(500, new { message = "An error occurred while adding the payment.", details = ex.ToString() });
            }
        }


        [HttpPut("{paymentId}")]
        public async Task<IActionResult> UpdatePayment(int paymentId, [FromBody] PaymentDTO paymentDto)
        {
            if (paymentDto == null)
            {
                _logger.LogWarning("UpdatePayment: Received null payment data for PaymentId {PaymentId}.", paymentId);
                return BadRequest("Payment data is null.");
            }

            try
            {
                // Validate if Order exists
                var order = await _orderService.GetOrderByIdAsync(paymentDto.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("UpdatePayment: Order with ID {OrderId} not found.", paymentDto.OrderId);
                    return NotFound($"Order with ID {paymentDto.OrderId} does not exist.");
                }

                // Set AmountToPay directly from TotalAmount of the order
                paymentDto.AmountToPay = order.TotalAmount;

                // Validate AmountToPay consistency
                if (paymentDto.AmountToPay != order.TotalAmount)
                {
                    _logger.LogWarning("UpdatePayment: AmountToPay mismatch for PaymentId {PaymentId}. Expected {ExpectedAmount}, but received {ReceivedAmount}.", paymentId, order.TotalAmount, paymentDto.AmountToPay);
                    return BadRequest($"AmountToPay mismatch. Expected: {order.TotalAmount}");
                }

                _logger.LogInformation("UpdatePayment: Updating payment with ID {PaymentId}.", paymentId);
                var payment = await _paymentService.UpdatePaymentAsync(paymentId, paymentDto);
                if (payment == null)
                {
                    _logger.LogWarning("UpdatePayment: Payment with ID {PaymentId} not found.", paymentId);
                    return NotFound("Payment not found.");
                }

                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response
                _logger.LogInformation("UpdatePayment: Successfully updated payment with ID {PaymentId}.", paymentId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePayment: An error occurred while updating payment with ID {PaymentId}.", paymentId);
                return BadRequest(new { message = "An error occurred while updating the payment." });
            }
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPayment(int paymentId)
        {
            try
            {
                _logger.LogInformation("GetPayment: Fetching payment with ID {PaymentId}.", paymentId);
                var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning("GetPayment: Payment with ID {PaymentId} not found.", paymentId);
                    return NotFound("Payment not found.");
                }

                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response
                _logger.LogInformation("GetPayment: Successfully fetched payment with ID {PaymentId}.", paymentId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayment: An error occurred while fetching payment with ID {PaymentId}.", paymentId);
                return StatusCode(500, new { message = "An error occurred while fetching the payment." });
            }
        }
    }
}
