using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("get-cart/{userId}")]
        public async Task<ActionResult<List<CartItemVm>>> GetCartItems(string userId)
        {
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            return Ok(cartItems);
        }

        [HttpPost("add-to-cart/{userId}")]
        public async Task<IActionResult> AddToCart(string userId, [FromBody] CartItemVm cartItem)
        {
            try
            {
                await _cartService.AddToCartAsync(userId, cartItem);
                return Ok(new { message = "Sản phẩm đã được thêm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng.", error = ex.Message });
            }

        }

        [HttpDelete("remove-from-cart/{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, Guid productId)
        {
            await _cartService.RemoveFromCartAsync(userId, productId);
            return Ok(new { message = "Sản phẩm đã được xóa khỏi giỏ hàng thành công!" });
        }

        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Giỏ hàng đã được xóa thành công!" });
        }
    }
}