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

        public PaymentController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDto)
        {
            try
            {
                var payment = await _paymentService.AddPaymentAsync(paymentDto);
                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response
                return CreatedAtAction(nameof(GetPayment), new { paymentId = payment.PaymentId }, paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{paymentId}")]
        public async Task<IActionResult> UpdatePayment(int paymentId, [FromBody] PaymentDTO paymentDto)
        {
            try
            {
                var payment = await _paymentService.UpdatePaymentAsync(paymentId, paymentDto);
                if (payment == null)
                {
                    return NotFound("Payment not found.");
                }
                var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPayment(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            var paymentResponse = _mapper.Map<PaymentDTO>(payment); // Map entity to DTO for response
            return Ok(paymentResponse);
        }
    }

}
