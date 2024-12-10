using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly AppDbContext _context; // Added to access products for price

        public OrderController(IOrderService orderService, IMapper mapper, ILogger<OrderController> logger, AppDbContext context)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
            _context = context;
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
                // Fetch the product to get the ProductPrice
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == orderDto.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("AddOrder: Product with ID {ProductId} not found.", orderDto.ProductId);
                    return BadRequest($"Product with ID {orderDto.ProductId} not found.");
                }

                // Set UnitPrice to ProductPrice from the product entity
                orderDto.UnitPrice = product.ProductPrice;

                // Validate TotalAmount consistency
                var expectedTotalAmount = orderDto.UnitPrice * orderDto.ItemQuantity;
                if (orderDto.TotalAmount != expectedTotalAmount)
                {
                    _logger.LogWarning("AddOrder: TotalAmount mismatch. Expected {ExpectedTotalAmount}, but received {ReceivedTotalAmount}.", expectedTotalAmount, orderDto.TotalAmount);
                    return BadRequest($"TotalAmount mismatch. Expected: {expectedTotalAmount}");
                }

                _logger.LogInformation("AddOrder: Adding a new order.");
                var order = await _orderService.AddOrderAsync(orderDto);
                if (order == null)
                    return BadRequest("Order could not be added");
                var orderResponse = _mapper.Map<OrderDTO>(order);

                _logger.LogInformation("AddOrder: Successfully added order with ID {OrderId}.", order.OrderId);
                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, orderDto);
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
                // Fetch the product to get the ProductPrice
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == orderDto.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("UpdateOrder: Product with ID {ProductId} not found.", orderDto.ProductId);
                    return BadRequest($"Product with ID {orderDto.ProductId} not found.");
                }

                // Set UnitPrice to ProductPrice from the product entity
                orderDto.UnitPrice = product.ProductPrice;

                // Validate TotalAmount consistency
                var expectedTotalAmount = orderDto.UnitPrice * orderDto.ItemQuantity;
                if (orderDto.TotalAmount != expectedTotalAmount)
                {
                    _logger.LogWarning("UpdateOrder: TotalAmount mismatch for OrderId {OrderId}. Expected {ExpectedTotalAmount}, but received {ReceivedTotalAmount}.", orderId, expectedTotalAmount, orderDto.TotalAmount);
                    return BadRequest($"TotalAmount mismatch. Expected: {expectedTotalAmount}");
                }

                _logger.LogInformation("UpdateOrder: Updating order with ID {OrderId}.", orderId);
                var order = await _orderService.UpdateOrderAsync(orderId, orderDto);
                if (order == null)
                {
                    _logger.LogWarning("UpdateOrder: Order with ID {OrderId} not found.", orderId);
                    return NotFound("Order not found.");
                }

                var orderResponse = _mapper.Map<OrderDTO>(order);
                _logger.LogInformation("UpdateOrder: Successfully updated order with ID {OrderId}.", orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateOrder: An error occurred while updating order with ID {OrderId}.", orderId);
                return NotFound(new { message = "An error occurred while updating the order." });
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
