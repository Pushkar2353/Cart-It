using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInventoryController : ControllerBase
    {
        private readonly IProductInventoryService _inventoryService;
        private readonly ILogger<ProductInventoryController> _logger;

        public ProductInventoryController(IProductInventoryService inventoryService, ILogger<ProductInventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpPost("add")]
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

        [HttpPut("update")]
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

        [HttpGet("{id}")]
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
    }
}
