using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<RegistrationController> _logger;
        private readonly ISellerService _sellerService;

        public RegistrationController(IMapper mapper, AppDbContext context, ICustomerService customerService, ISellerService sellerService, ILogger<RegistrationController> logger) //IReviewService reviewService, IPaymentService paymentService, IOrderService orderService, ICartService cartService,IProductService productService, ICategoryService categoryService,
        {
            _customerService = customerService;
            _sellerService = sellerService;
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        // POST: api/customer
        [HttpPost("CustomerRegistration")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                {
                    _logger.LogWarning("Customer data is null.");
                    return BadRequest(new { Message = "Customer data cannot be null." });
                }

                _logger.LogInformation("Attempting to create a customer with email {CustomerEmail}", customerDto.Email);

                var createdCustomer = await _customerService.AddCustomerAsync(customerDto);

                _logger.LogInformation("Customer with ID {CustomerId} created successfully", createdCustomer.CustomerId);
                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId },
                new { Message = "Customer created successfully.", Customer = createdCustomer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a customer.");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetCust")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching customer with ID {CustomerId}", id);

                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    return NotFound(new { Message = $"Customer with ID {id} not found." });
                }

                _logger.LogInformation("Customer with ID {CustomerId} found", id);
                return Ok(new { Message = "Customer found successfully.", Customer = customer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customer with ID {CustomerId}", id);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // POST: api/seller
        [HttpPost("SellerRegistration")]
        public async Task<ActionResult<SellerDTO>> CreateSeller([FromBody] SellerDTO sellerDto)
        {
            try
            {
                if (sellerDto == null)
                {
                    _logger.LogWarning("Invalid seller data received for creation.");
                    return BadRequest(new { message = "Invalid seller data" });
                }

                _logger.LogInformation("Creating new seller with email {SellerEmail}", sellerDto.Email);

                var createdSeller = await _sellerService.CreateSellerAsync(sellerDto);

                _logger.LogInformation("Seller with ID {SellerId} created successfully", createdSeller.SellerId);
                return CreatedAtAction(nameof(GetSellerById), new { id = createdSeller.SellerId }, createdSeller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new seller.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetSell")]
        public async Task<ActionResult<SellerDTO>> GetSellerById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching seller with ID {SellerId}", id);

                var seller = await _sellerService.GetSellerByIdAsync(id);

                if (seller == null)
                {
                    _logger.LogWarning("Seller with ID {SellerId} not found", id);
                    return NotFound(new { message = "Seller not found" });
                }

                _logger.LogInformation("Seller with ID {SellerId} found", id);
                return Ok(seller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching seller with ID {SellerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
