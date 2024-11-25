using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.WebApi.ViewModels;
using System.Security.Claims;

namespace SportStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUserService userService, UserManager<AppUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
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
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfile updateProfile)
        {
            if (updateProfile == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            if (updateProfile.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized("Không thể cập nhật tài khoản của người dùng khác.");
            }

            try
            {
                var currentUser = await _userService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return NotFound("Không tìm thấy tài khoản hiện tại.");
                }

                await _userService.UpdateCurrentUserAsync(updateProfile);
                return Ok(new { message = "Cập nhật thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized(new { message = "Không tìm thấy người dùng." });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Đã thay đổi mật khẩu thành công." });
        }

        [HttpPost("mark-for-deletion")]
        public async Task<IActionResult> MarkForDeletion()
        {
            try
            {
                await _userService.MarkForDeletionAsync();
                return Ok(new { message = "Tài khoản đã được đánh dấu xóa." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("restore-account")]
        public async Task<IActionResult> RestoreAccount()
        {
            try
            {
                await _userService.RestoreAccountAsync();
                return Ok(new { message = "Tài khoản đã được khôi phục." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("account-status")]
        public async Task<IActionResult> GetAccountStatus()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "Tài khoản không tồn tại." });
                }

                bool isDeleted = user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow;

                return Ok(new { isDeleted, deletionDate = user.LockoutEnd });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}