using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]

    public class AdministratorController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;
        private readonly ICustomerService _customerService;
        private readonly ISellerService _sellerService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IProductInventoryService _inventoryService;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        private readonly ILogger<AdministratorController> _logger;

        public AdministratorController(IAdministratorService administratorService, IReviewService reviewService, IOrderService orderService, IPaymentService paymentService, ICartService cartService, IProductInventoryService inventoryService, IProductService productService, ICategoryService categoryService, ISellerService sellerService, ICustomerService customerService, IMapper mapper, ILogger<AdministratorController> logger)
        {
            _administratorService = administratorService;
            _customerService = customerService;
            _sellerService = sellerService;
            _categoryService = categoryService;
            _productService = productService;
            _inventoryService = inventoryService;
            _cartService = cartService;
            _paymentService = paymentService;
            _orderService = orderService;
            _reviewService = reviewService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/administrator/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AdministratorDTO>> GetAdministratorById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching administrator with ID {AdminId}", id);

                var administrator = await _administratorService.GetAdministratorByIdAsync(id);

                if (administrator == null)
                {
                    _logger.LogWarning("Administrator with ID {AdminId} not found", id);
                    return NotFound(new { message = "Administrator not found" });
                }

                _logger.LogInformation("Administrator with ID {AdminId} fetched successfully", id);
                return Ok(administrator);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching administrator with ID {AdminId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/administrator
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdministratorDTO>>> GetAllAdministrators()
        {
            try
            {
                _logger.LogInformation("Fetching all administrators");

                var administrators = await _administratorService.GetAllAdministratorsAsync();

                _logger.LogInformation("Successfully fetched {AdminCount} administrators", administrators.Count());
                return Ok(administrators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all administrators");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/administrator
        [HttpPost]
        public async Task<ActionResult<AdministratorDTO>> CreateAdministrator([FromBody] AdministratorDTO administratorDto)
        {
            try
            {
                if (administratorDto == null)
                {
                    _logger.LogWarning("Invalid administrator data received for creation");
                    return BadRequest(new { message = "Invalid administrator data" });
                }

                _logger.LogInformation("Creating new administrator with email {AdminEmail}", administratorDto.Email);

                var createdAdministrator = await _administratorService.CreateAdministratorAsync(administratorDto);

                _logger.LogInformation("Administrator with ID {AdminId} created successfully", createdAdministrator.AdminId);
                return CreatedAtAction(nameof(GetAdministratorById), new { id = createdAdministrator.AdminId }, createdAdministrator);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new administrator");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH: api/administrator/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult<AdministratorDTO>> UpdateAdministrator(int id, [FromBody] AdministratorDTO administratorDto)
        {
            try
            {
                if (administratorDto == null)
                {
                    _logger.LogWarning("Invalid administrator data received for update, ID: {AdminId}", id);
                    return BadRequest(new { message = "Invalid administrator data" });
                }

                _logger.LogInformation("Updating administrator with ID {AdminId}", id);

                var updatedAdministrator = await _administratorService.UpdateAdministratorAsync(id, administratorDto);

                if (updatedAdministrator == null)
                {
                    _logger.LogWarning("Administrator with ID {AdminId} not found for update", id);
                    return NotFound(new { message = "Administrator not found" });
                }

                _logger.LogInformation("Administrator with ID {AdminId} updated successfully", id);
                return Ok(updatedAdministrator);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating administrator with ID {AdminId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/administrator/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdministrator(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete administrator with ID {AdminId}", id);

                var result = await _administratorService.DeleteAdministratorAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Administrator with ID {AdminId} not found for deletion", id);
                    return NotFound(new { message = "Administrator not found" });
                }

                _logger.LogInformation("Administrator with ID {AdminId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting administrator with ID {AdminId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
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

        // GET: api/seller/{id}
        [HttpGet("GetSellerById/{id}")]
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

        // GET: api/seller
        [HttpGet("GetAllSellers")]
        public async Task<ActionResult<IEnumerable<SellerDTO>>> GetAllSellers()
        {
            try
            {
                _logger.LogInformation("Fetching all sellers.");

                var sellers = await _sellerService.GetAllSellersAsync();

                _logger.LogInformation("Successfully fetched {SellerCount} sellers", sellers.Count());
                return Ok(sellers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all sellers.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/seller/{id}
        [HttpDelete("DeleteSeller/{id}")]
        public async Task<ActionResult> DeleteSeller(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete seller with ID {SellerId}", id);

                var result = await _sellerService.DeleteSellerAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Seller with ID {SellerId} not found for deletion", id);
                    return NotFound(new { message = "Seller not found" });
                }

                _logger.LogInformation("Seller with ID {SellerId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting seller with ID {SellerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories.");

                var categories = await _categoryService.GetCategoriesAsync();

                _logger.LogInformation("Successfully fetched {Count} categories.", categories.Count());
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching category with ID {CategoryId}.", id);

                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", id);
                    return NotFound(new { message = "Category not found" });
                }

                _logger.LogInformation("Successfully fetched category with ID {CategoryId}.", id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category with ID {CategoryId}.", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto == null)
                {
                    _logger.LogWarning("Received null data for category creation.");
                    return BadRequest(new { message = "Invalid category data." });
                }

                _logger.LogInformation("Creating new category with name {CategoryName}.", categoryDto.CategoryName);

                var createdCategory = await _categoryService.AddCategoryAsync(categoryDto);

                _logger.LogInformation("Category with ID {CategoryId} created successfully.", createdCategory.CategoryId);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new category.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto == null)
                {
                    _logger.LogWarning("Received null data for updating category with ID {CategoryId}.", id);
                    return BadRequest(new { message = "Invalid category data." });
                }

                _logger.LogInformation("Updating category with ID {CategoryId}.", id);

                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);

                _logger.LogInformation("Category with ID {CategoryId} updated successfully.", id);
                return Ok(updatedCategory);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for update.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID {CategoryId}.", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete category with ID {CategoryId}.", id);

                await _categoryService.DeleteCategoryAsync(id);

                _logger.LogInformation("Category with ID {CategoryId} deleted successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID {CategoryId}.", id);
                return StatusCode(500, new { message = ex.Message });
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

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID {ProductId}.", id);

                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", id);
                    return NotFound(new { message = "Product not found" });
                }

                _logger.LogInformation("Successfully fetched product with ID {ProductId}.", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product with ID {ProductId}.", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product with ID {ProductId}.", id);

                await _productService.DeleteProductAsync(id);

                _logger.LogInformation("Product with ID {ProductId} deleted successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}.", id);
                return StatusCode(500, new { message = ex.Message });
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

        [HttpGet("GetInventoryById/{id}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching inventory with ID {InventoryId}.", id);

                var inventory = await _inventoryService.GetInventoryByIdAsync(id);

                if (inventory == null)
                {
                    _logger.LogWarning("Inventory with ID {InventoryId} not found.", id);
                    return NotFound(new { message = "Inventory not found" });
                }

                _logger.LogInformation("Successfully fetched inventory with ID {InventoryId}.", id);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching inventory with ID {InventoryId}.", id);
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

        [HttpGet("GetAllCarts")]
        public async Task<IActionResult> GetAllCarts()
        {
            _logger.LogInformation("Fetching all carts.");

            var carts = await _cartService.GetAllCartsAsync();

            _logger.LogInformation("Successfully fetched {Count} carts.", carts.Count());
            return Ok(carts);
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

        // GET: api/review/{reviewId}
        [HttpGet("GetReview/{reviewId}")]
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

        // GET: api/review/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            try
            {
                _logger.LogInformation("GetReviewsByProductId: Fetching reviews for ProductId {ProductId}.", productId);
                var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);

                if (reviews == null || !reviews.Any())
                {
                    _logger.LogWarning("GetReviewsByProductId: No reviews found for ProductId {ProductId}.", productId);
                    return NotFound("No reviews found for this product.");
                }

                var reviewResponses = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
                _logger.LogInformation("GetReviewsByProductId: Successfully fetched {Count} reviews for ProductId {ProductId}.", reviewResponses.Count(), productId);
                return Ok(reviewResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReviewsByProductId: An error occurred while fetching reviews for ProductId {ProductId}.", productId);
                return StatusCode(500, new { message = "An error occurred while fetching the reviews." });
            }
        }
    }
}
