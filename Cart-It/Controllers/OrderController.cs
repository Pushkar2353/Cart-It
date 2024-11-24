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

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO orderDto)
        {
            try
            {
                var order = await _orderService.AddOrderAsync(orderDto);
                var orderResponse = _mapper.Map<OrderDTO>(order);
                return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, orderResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderDTO orderDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderAsync(orderId, orderDto);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }
                var orderResponse = _mapper.Map<OrderDTO>(order);
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var orderResponse = _mapper.Map<OrderDTO>(order);
            return Ok(orderResponse);
        }
    }
}
