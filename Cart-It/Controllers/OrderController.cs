using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IMapper mapper, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
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

        [HttpPut("{orderId}")]
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

        [HttpGet("{orderId}")]
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
    }
}
