using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
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

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
    }
}