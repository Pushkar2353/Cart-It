using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound(new { Message = $"Customer with ID {id} not found." });

                return Ok(new { Message = "Customer found successfully.", Customer = customer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // GET: api/customer
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(new { Message = "All customers fetched successfully.", Customers = customers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                    return BadRequest(new { Message = "Customer data cannot be null." });

                // Debugging line to check the incoming DTO
                Console.WriteLine(customerDto);

                var createdCustomer = await _customerService.AddCustomerAsync(customerDto);
                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId },
                new { Message = "Customer created successfully.", Customer = createdCustomer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // PATCH: api/customer/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCustomerPartial(int id, [FromBody] CustomerDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                    return BadRequest(new { Message = "Customer data cannot be null." });

                await _customerService.UpdateCustomerPartialAsync(id, customerDto);
                return Ok(new { Message = "Customer updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                return Ok(new { Message = "Customer deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
