using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using System.Security.Claims;

namespace SportStore.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public UserOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderVm>> GetOrderById(Guid orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Người dùng chưa được xác thực.");
            }

            var order = await _orderService.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
                return NotFound($"Không tìm thấy đơn hàng có ID {orderId} hoặc đơn hàng đó không thuộc về người dùng.");
            }

            return Ok(order);
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Người dùng chưa được xác thực.");
            }

            try
            {
                var canCancel = await _orderService.CanCancelOrderAsync(orderId, userId);
                if (!canCancel)
                {
                    return BadRequest("Không thể hủy đơn hàng.");
                }

                await _orderService.CancelOrderAsync(orderId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi không mong muốn.");
            }
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderVm>> PlaceOrder(OrderVm orderVm)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Người dùng chưa được xác thực.");
            }

            try
            {
                var order = await _orderService.CheckoutAsync(orderVm, userId);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi không mong muốn khi đặt hàng.");
            }
        }
    }
}