using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.WebApi.ViewModels;

namespace SportStore.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("get-user-by-id/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
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

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.DeleteUserByIdAsync(id);
                return Ok("Xóa thành công.");
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

        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUser(string id)
        {
            try
            {
                await _userService.LockUserAsync(id);
                return Ok($"Tài khoản {id} đã bị khóa.");
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

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            try
            {
                await _userService.UnlockUserAsync(id);
                return Ok("Tài khoản đã được mở khóa.");
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

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentVm roleAssignment)
        {
            try
            {
                await _userService.AssignRoleToUserAsync(roleAssignment.UserId, roleAssignment.RoleName);
                return Ok($"Đăng ký quyền cho {roleAssignment.UserId} thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}