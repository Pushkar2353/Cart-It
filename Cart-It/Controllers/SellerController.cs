using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        // GET: api/seller/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SellerDTO>> GetSellerById(int id)
        {
            var seller = await _sellerService.GetSellerByIdAsync(id);

            if (seller == null)
            {
                return NotFound(new { message = "Seller not found" });
            }

            return Ok(seller);
        }

        // GET: api/seller
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellerDTO>>> GetAllSellers()
        {
            var sellers = await _sellerService.GetAllSellersAsync();
            return Ok(sellers);
        }

        // POST: api/seller
        [HttpPost]
        public async Task<ActionResult<SellerDTO>> CreateSeller([FromBody] SellerDTO sellerDto)
        {
            if (sellerDto == null)
            {
                return BadRequest(new { message = "Invalid seller data" });
            }

            var createdSeller = await _sellerService.CreateSellerAsync(sellerDto);

            return CreatedAtAction(nameof(GetSellerById), new { id = createdSeller.SellerId }, createdSeller);
        }

        // PATCH: api/seller/{id} - Update seller partially
        [HttpPatch("{id}")]
        public async Task<ActionResult<SellerDTO>> UpdateSeller(int id, [FromBody] SellerDTO sellerDto)
        {
            if (sellerDto == null)
            {
                return BadRequest(new { message = "Invalid seller data" });
            }

            // Call the service to update the seller (this can handle partial updates)
            var updatedSeller = await _sellerService.UpdateSellerAsync(id, sellerDto);

            if (updatedSeller == null)
            {
                return NotFound(new { message = "Seller not found" });
            }

            return Ok(updatedSeller);
        }

        // DELETE: api/seller/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSeller(int id)
        {
            var result = await _sellerService.DeleteSellerAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Seller not found" });
            }

            return NoContent();
        }
    }
}
