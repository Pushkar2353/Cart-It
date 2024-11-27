using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]

    public class CustomerController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly AppDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public CustomerController(IReviewService reviewService, IPaymentService paymentService, IOrderService orderService, IMapper mapper, AppDbContext context,ICartService cartService,IProductService productService, ICustomerService customerService, ICategoryService categoryService, ILogger<CustomerController> logger)
        {
            _reviewService = reviewService;
            _cartService = cartService;
            _productService = productService;
            _customerService = customerService;
            _categoryService = categoryService;
            _logger = logger;
            _context = context;
            _orderService = orderService;
            _mapper = mapper;
            _paymentService = paymentService;

        }

        // GET: api/customer/{id}
        [HttpGet("GetCustomerById/{id}")]
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

        // GET: api/customer
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                _logger.LogInformation("Fetching all customers.");

                var customers = await _customerService.GetAllCustomersAsync();
                _logger.LogInformation("Successfully fetched {CustomerCount} customers", customers.Count());

                return Ok(new { Message = "All customers fetched successfully.", Customers = customers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all customers.");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("RegisterCustomer")]
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

        // PATCH: api/customer/{id}
        [HttpPatch("UpdateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomerPartial(int id, [FromBody] CustomerDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                {
                    _logger.LogWarning("Update data for customer with ID {CustomerId} is null", id);
                    return BadRequest(new { Message = "Customer data cannot be null." });
                }

                _logger.LogInformation("Updating customer with ID {CustomerId}", id);

                await _customerService.UpdateCustomerPartialAsync(id, customerDto);

                _logger.LogInformation("Customer with ID {CustomerId} updated successfully", id);
                return Ok(new { Message = "Customer updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found for update", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer with ID {CustomerId}", id);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // DELETE: api/customer/{id}
        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete customer with ID {CustomerId}", id);

                await _customerService.DeleteCustomerAsync(id);

                _logger.LogInformation("Customer with ID {CustomerId} deleted successfully", id);
                return Ok(new { Message = "Customer deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting customer with ID {CustomerId}", id);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories.");

                var categories = await _categoryService.GetCategoriesAsync();

                _logger.LogInformation("Successfully fetched {CategoryCount} categories", categories.Count());
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories.");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            _logger.LogInformation($"Received request to get products for category ID: {categoryId}");

            try
            {
                var products = await _productService.GetProductsByCategoryIdAsync(categoryId);

                if (products == null || !products.Any())
                {
                    _logger.LogWarning($"No products found for category ID: {categoryId}");
                    return NotFound("No products found for the given category.");
                }

                _logger.LogInformation($"Successfully retrieved {products.Count()} products for category ID: {categoryId}");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching products for category ID: {categoryId}. Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("Fetching all products.");

                var products = await _productService.GetAllProductsAsync();

                _logger.LogInformation("Successfully fetched {Count} products.", products.Count());
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all products.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetCartById/{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            _logger.LogInformation("Fetching cart with ID {CartId}.", id);

            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                _logger.LogWarning("Cart with ID {CartId} not found.", id);
                return NotFound(new { message = "Cart not found." });
            }

            _logger.LogInformation("Successfully fetched cart with ID {CartId}.", id);
            return Ok(cart);
        }

        [HttpPost("AddCart")]
        public async Task<IActionResult> AddCart([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
            {
                _logger.LogWarning("Received null cart data.");
                return BadRequest(new { message = "Cart data is null." });
            }

            try
            {
                _logger.LogInformation("Checking if customer with ID {CustomerId} exists.", cartDto.CustomerId);
                var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == cartDto.CustomerId);
                if (!customerExists)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} does not exist.", cartDto.CustomerId);
                    return BadRequest(new { message = "Customer does not exist." });
                }

                _logger.LogInformation("Checking if product with ID {ProductId} exists.", cartDto.ProductId);
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == cartDto.ProductId);
                if (!productExists)
                {
                    _logger.LogWarning("Product with ID {ProductId} does not exist.", cartDto.ProductId);
                    return BadRequest(new { message = "Product does not exist." });
                }

                _logger.LogInformation("Adding cart for customer ID {CustomerId} and product ID {ProductId}.", cartDto.CustomerId, cartDto.ProductId);
                await _cartService.AddCartAsync(cartDto);

                _logger.LogInformation("Successfully added cart with ID {CartId}.", cartDto.CartId);
                return CreatedAtAction(nameof(GetCartById), new { id = cartDto.CartId }, cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding cart.");
                return StatusCode(500, new { message = "An error occurred while adding the cart." });
            }
        }

        [HttpPut("UpdateCart/{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] CartDTO cartDto)
        {
            if (cartDto == null || id != cartDto.CartId)
            {
                _logger.LogWarning("Invalid data for updating cart with ID {CartId}.", id);
                return BadRequest(new { message = "Invalid cart data." });
            }

            try
            {
                _logger.LogInformation("Updating cart with ID {CartId}.", id);
                await _cartService.UpdateCartAsync(id, cartDto);

                _logger.LogInformation("Successfully updated cart with ID {CartId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart with ID {CartId}.", id);
                return StatusCode(500, new { message = "An error occurred while updating the cart." });
            }
        }

        [HttpDelete("DeleteCart/{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            try
            {
                _logger.LogInformation("Deleting cart with ID {CartId}.", id);
                await _cartService.DeleteCartAsync(id);

                _logger.LogInformation("Successfully deleted cart with ID {CartId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting cart with ID {CartId}.", id);
                return StatusCode(500, new { message = "An error occurred while deleting the cart." });
            }
        }


        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO orderDto)
        {
            if (orderDto == null)
            {
                _logger.LogWarning("AddOrder: Received null order data.");
                return BadRequest("Order data is null.");
            }

            try
            {
                _logger.LogInformation("AddOrder: Adding a new order.");
                var order = await _orderService.AddOrderAsync(orderDto);
                var orderResponse = _mapper.Map<OrderDTO>(order);

                _logger.LogInformation("AddOrder: Successfully added order with ID {OrderId}.", order.OrderId);
                return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, orderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddOrder: An error occurred while adding a new order.");
                return BadRequest(new { message = "An error occurred while adding the order." });
            }
        }

        [HttpPut("UpdateOrder/{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderDTO orderDto)
        {
            if (orderDto == null)
            {
                _logger.LogWarning("UpdateOrder: Received null order data for OrderId {OrderId}.", orderId);
                return BadRequest("Order data is null.");
            }

            try
            {
                _logger.LogInformation("UpdateOrder: Updating order with ID {OrderId}.", orderId);
                var order = await _orderService.UpdateOrderAsync(orderId, orderDto);
                if (order == null)
                {
                    _logger.LogWarning("UpdateOrder: Order with ID {OrderId} not found.", orderId);
                    return NotFound("Order not found.");
                }

                var orderResponse = _mapper.Map<OrderDTO>(order);
                _logger.LogInformation("UpdateOrder: Successfully updated order with ID {OrderId}.", orderId);
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateOrder: An error occurred while updating order with ID {OrderId}.", orderId);
                return BadRequest(new { message = "An error occurred while updating the order." });
            }
        }

        [HttpGet("GetOrder/{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            try
            {
                _logger.LogInformation("GetOrder: Fetching order with ID {OrderId}.", orderId);
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("GetOrder: Order with ID {OrderId} not found.", orderId);
                    return NotFound("Order not found.");
                }

                var orderResponse = _mapper.Map<OrderDTO>(order);
                _logger.LogInformation("GetOrder: Successfully fetched order with ID {OrderId}.", orderId);
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOrder: An error occurred while fetching order with ID {OrderId}.", orderId);
                return StatusCode(500, new { message = "An error occurred while fetching the order." });
            }
        }

        [HttpPost("AddPayment")]
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

        [HttpPut("UpdatePayment/{paymentId}")]
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

        [HttpGet("GetPayment/{paymentId}")]
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

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDto)
        {
            if (reviewDto == null)
            {
                _logger.LogWarning("AddReview: Received null review data.");
                return BadRequest("Review data is null.");
            }

            try
            {
                _logger.LogInformation("AddReview: Adding a new review.");
                var review = await _reviewService.AddReviewAsync(reviewDto);
                var reviewResponse = _mapper.Map<ReviewDTO>(review);

                _logger.LogInformation("AddReview: Successfully added review with ID {ReviewId}.", review.ReviewId);
                return CreatedAtAction(nameof(GetReview), new { reviewId = review.ReviewId }, reviewResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddReview: An error occurred while adding a new review.");
                return BadRequest(new { message = "An error occurred while adding the review." });
            }
        }

        [HttpGet("ViewReview/{reviewId}")]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            try
            {
                _logger.LogInformation("GetReview: Fetching review with ID {ReviewId}.", reviewId);
                var review = await _reviewService.GetReviewByIdAsync(reviewId);

                if (review == null)
                {
                    _logger.LogWarning("GetReview: Review with ID {ReviewId} not found.", reviewId);
                    return NotFound("Review not found.");
                }

                var reviewResponse = _mapper.Map<ReviewDTO>(review);
                _logger.LogInformation("GetReview: Successfully fetched review with ID {ReviewId}.", reviewId);
                return Ok(reviewResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReview: An error occurred while fetching review with ID {ReviewId}.", reviewId);
                return StatusCode(500, new { message = "An error occurred while fetching the review." });
            }
        }
    }
}
