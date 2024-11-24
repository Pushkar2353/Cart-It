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

        public ProductInventoryController(IProductInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromBody] ProductInventoryDTO inventoryDto)
        {
            try
            {
                var addedInventory = await _inventoryService.AddInventoryAsync(inventoryDto);
                return CreatedAtAction(nameof(GetInventoryById), new { id = addedInventory.InventoryId }, addedInventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateInventory([FromBody] ProductInventoryDTO inventoryDto)
        {
            try
            {
                var updatedInventory = await _inventoryService.UpdateInventoryAsync(inventoryDto);
                return Ok(updatedInventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByIdAsync(id);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

}
