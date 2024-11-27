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

        public PaymentController(IPaymentService paymentService, IMapper mapper, ILogger<PaymentController> logger)
        {
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
                _logger.LogInformation("AddPayment: Adding a new payment.");
                var payment = await _paymentService.AddPaymentAsync(paymentDto);
                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response

                _logger.LogInformation("AddPayment: Successfully added payment with ID {PaymentId}.", payment.PaymentId);
                return CreatedAtAction(nameof(GetPayment), new { paymentId = payment.PaymentId }, paymentResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPayment: An error occurred while adding a new payment.");
                return BadRequest(new { message = "An error occurred while adding the payment." });
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
                _logger.LogInformation("UpdatePayment: Updating payment with ID {PaymentId}.", paymentId);
                var payment = await _paymentService.UpdatePaymentAsync(paymentId, paymentDto);
                if (payment == null)
                {
                    _logger.LogWarning("UpdatePayment: Payment with ID {PaymentId} not found.", paymentId);
                    return NotFound("Payment not found.");
                }

                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response
                _logger.LogInformation("UpdatePayment: Successfully updated payment with ID {PaymentId}.", paymentId);
                return Ok(paymentResponse);
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
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayment: An error occurred while fetching payment with ID {PaymentId}.", paymentId);
                return StatusCode(500, new { message = "An error occurred while fetching the payment." });
            }
        }
    }
}
