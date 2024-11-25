using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.WebApi.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;

        public AdminOrdersController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderVm>>> GetAllOrders()
        {
            var orders = await _adminOrderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderVm>> GetOrderById(Guid orderId)
        {
            var order = await _adminOrderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound($"Không tìm thấy đơn hàng có ID {orderId}.");
            return Ok(order);
        }

        [HttpPut("{orderId}/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusVm updateStatusVm)
        {
            try
            {
                await _adminOrderService.UpdateOrderStatusAsync(orderId, updateStatusVm);
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
    }
}