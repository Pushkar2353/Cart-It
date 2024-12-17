using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
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

        [HttpPost]
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

        // Update product
        [HttpPut("{id}")]
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

                if (updatedProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update.", id);
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

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

        // Delete product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product with ID {ProductId}.", id);

                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion.", id);
                    return NotFound(new { message = $"Product not found" });
                }

                await _productService.DeleteProductAsync(id);

                _logger.LogInformation("Product with ID {ProductId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the product: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
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
    }
}
