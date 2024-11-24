using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;

        public AdministratorController(IAdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

        // GET: api/administrator/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AdministratorDTO>> GetAdministratorById(int id)
        {
            var administrator = await _administratorService.GetAdministratorByIdAsync(id);

            if (administrator == null)
            {
                return NotFound(new { message = "Administrator not found" });
            }

            return Ok(administrator);
        }

        // GET: api/administrator
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdministratorDTO>>> GetAllAdministrators()
        {
            var administrators = await _administratorService.GetAllAdministratorsAsync();
            return Ok(administrators);
        }

        // POST: api/administrator
        [HttpPost]
        public async Task<ActionResult<AdministratorDTO>> CreateAdministrator([FromBody] AdministratorDTO administratorDto)
        {
            if (administratorDto == null)
            {
                return BadRequest(new { message = "Invalid administrator data" });
            }

            var createdAdministrator = await _administratorService.CreateAdministratorAsync(administratorDto);

            return CreatedAtAction(nameof(GetAdministratorById), new { id = createdAdministrator.AdminId }, createdAdministrator);
        }

        // PATCH: api/administrator/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult<AdministratorDTO>> UpdateAdministrator(int id, [FromBody] AdministratorDTO administratorDto)
        {
            if (administratorDto == null)
            {
                return BadRequest(new { message = "Invalid administrator data" });
            }

            var updatedAdministrator = await _administratorService.UpdateAdministratorAsync(id, administratorDto);

            if (updatedAdministrator == null)
            {
                return NotFound(new { message = "Administrator not found" });
            }

            return Ok(updatedAdministrator);
        }

        // DELETE: api/administrator/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdministrator(int id)
        {
            var result = await _administratorService.DeleteAdministratorAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Administrator not found" });
            }

            return NoContent();
        }
    }

}
