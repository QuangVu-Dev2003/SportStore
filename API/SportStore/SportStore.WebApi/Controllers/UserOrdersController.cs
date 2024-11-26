using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;

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

            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId, userId);
                var orderVm = new OrderVm
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    ShippingAddress = order.ShippingAddress,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailVm
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product?.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.Product?.Price ?? 0
                    }).ToList()
                };

                return Ok(orderVm);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Người dùng chưa được xác thực." });
            }

            try
            {
                var canCancel = await _orderService.CanCancelOrderAsync(orderId, userId);
                if (!canCancel)
                {
                    return BadRequest(new { message = "Không thể hủy đơn hàng." });
                }

                var success = await _orderService.CancelOrderAsync(orderId, userId);
                if (success)
                {
                    return Ok(new { message = "Hủy đơn hàng thành công.", status = "Cancelled" });
                }

                return BadRequest(new { message = "Không thể hủy đơn hàng." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Đã xảy ra lỗi không mong muốn.", details = ex.Message });
            }
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderVm>> PlaceOrder([FromBody] OrderVm orderVm)
        {
            if (orderVm == null || orderVm.OrderDetails == null || !orderVm.OrderDetails.Any())
            {
                Console.WriteLine("Dữ liệu nhận được không hợp lệ: " + JsonConvert.SerializeObject(orderVm));
                return BadRequest("Dữ liệu không hợp lệ hoặc thiếu thông tin đơn hàng.");
            }

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
                Console.WriteLine("Lỗi khi xử lý đơn hàng: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi không mong muốn khi đặt hàng.");
            }
        }
    }
}