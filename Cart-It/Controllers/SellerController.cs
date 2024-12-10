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
    [Authorize(Roles = "Seller")]

    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;
        private readonly IMapper _mapper;
        private readonly ILogger<SellerController> _logger;

        /*
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IProductInventoryService _inventoryService;
        private readonly IReviewService _reviewService;

        */

        public SellerController(ISellerService sellerService, IMapper mapper, ILogger<SellerController> logger) //IReviewService reviewService, IProductInventoryService inventoryService, IProductService productService, ICategoryService categoryService,
        {
            _sellerService = sellerService;
            /*
            _categoryService = categoryService;
            _productService = productService;
            _inventoryService = inventoryService;
            _reviewService = reviewService;
            */
            _mapper = mapper;
            _logger = logger;
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

        // POST: api/seller
        [HttpPost("CreateSeller")]
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

        // PATCH: api/seller/{id} - Update seller partially
        [HttpPatch("UpdateSeller/{id}")]
        public async Task<ActionResult<SellerDTO>> UpdateSeller(int id, [FromBody] SellerDTO sellerDto)
        {
            try
            {
                if (sellerDto == null)
                {
                    _logger.LogWarning("Invalid seller data received for update, ID: {SellerId}", id);
                    return BadRequest(new { message = "Invalid seller data" });
                }

                _logger.LogInformation("Updating seller with ID {SellerId}", id);

                // Call the service to update the seller (this can handle partial updates)
                var updatedSeller = await _sellerService.UpdateSellerAsync(id, sellerDto);

                if (updatedSeller == null)
                {
                    _logger.LogWarning("Seller with ID {SellerId} not found for update", id);
                    return NotFound(new { message = "Seller not found" });
                }

                _logger.LogInformation("Seller with ID {SellerId} updated successfully", id);
                return Ok(updatedSeller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating seller with ID {SellerId}", id);
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

        /*

        [HttpGet("GetCategories")]
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

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _logger.LogWarning("Received null data for product creation.");
                    return BadRequest(new { message = "Invalid product data." });
                }

                _logger.LogInformation("Adding a new product with name {ProductName}.", productDto.ProductName);

                var addedProduct = await _productService.AddProductAsync(productDto);

                _logger.LogInformation("Product with ID {ProductId} created successfully.", addedProduct.ProductId);
                return CreatedAtAction(nameof(GetProductById), new { id = addedProduct.ProductId }, addedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new product.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _logger.LogWarning("Received null data for updating product with ID {ProductId}.", id);
                    return BadRequest(new { message = "Invalid product data." });
                }

                _logger.LogInformation("Updating product with ID {ProductId}.", id);

                var updatedProduct = await _productService.UpdateProductAsync(id, productDto);

                _logger.LogInformation("Product with ID {ProductId} updated successfully.", id);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}.", id);
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


        [HttpPost("AddInventory")]
        public async Task<IActionResult> AddInventory([FromBody] ProductInventoryDTO inventoryDto)
        {
            try
            {
                _logger.LogInformation("Attempting to add inventory for product ID {ProductId}.", inventoryDto.ProductId);

                var addedInventory = await _inventoryService.AddInventoryAsync(inventoryDto);

                _logger.LogInformation("Successfully added inventory with ID {InventoryId} for product ID {ProductId}.", addedInventory.InventoryId, inventoryDto.ProductId);
                return CreatedAtAction(nameof(GetInventoryById), new { id = addedInventory.InventoryId }, addedInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding inventory for product ID {ProductId}.", inventoryDto?.ProductId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateInventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] ProductInventoryDTO inventoryDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update inventory with ID {InventoryId}.", inventoryDto.InventoryId);

                var updatedInventory = await _inventoryService.UpdateInventoryAsync(inventoryDto);

                _logger.LogInformation("Successfully updated inventory with ID {InventoryId}.", inventoryDto.InventoryId);
                return Ok(updatedInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating inventory with ID {InventoryId}.", inventoryDto?.InventoryId);
                return BadRequest(new { message = ex.Message });
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
        */
    }
}
