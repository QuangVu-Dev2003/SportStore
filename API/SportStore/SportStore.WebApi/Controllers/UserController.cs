using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.DataAccessLayer.Models;
using System.Security.Claims;

namespace SportStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userName = User.Identity?.Name;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            Console.WriteLine($"User: {userName}, Roles: {string.Join(", ", roles)}");

            var user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] AppUser userUpdate)
        {
            try
            {
                await _userService.UpdateCurrentUserAsync(userUpdate);
                return Ok("Cập nhật thành công.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("mark-for-deletion")]
        public async Task<IActionResult> MarkForDeletion()
        {
            await _userService.MarkForDeletionAsync();
            return Ok("Tài khoản được đánh dấu để xóa.");
        }

        [HttpPost("restore-account")]
        public async Task<IActionResult> RestoreAccount()
        {
            await _userService.RestoreAccountAsync();
            return Ok("Khôi phục thành công.");
        }
    }
}