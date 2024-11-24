﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;
using System.Security.Claims;

namespace SportStore.BusinessLogicLayer.Services
{
    public class UserService : BaseService<AppUser>, IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<bool> IsCurrentUserAdminAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        // Normal User
        public async Task<AppUser> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task UpdateCurrentUserAsync(AppUser user)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser.Id == user.Id)
            {
                await _userManager.UpdateAsync(user);
            }
            else
            {
                throw new UnauthorizedAccessException("Không thể cập nhật tài khoản của người dùng khác.");
            }
        }

        public async Task MarkForDeletionAsync()
        {
            var user = await GetCurrentUserAsync();
            await UpdateCurrentUserAsync(user);
        }

        public async Task RestoreAccountAsync()
        {
            var user = await GetCurrentUserAsync();
            await UpdateCurrentUserAsync(user);
        }

        // Admin
        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            if (!await IsCurrentUserAdminAsync())
                throw new UnauthorizedAccessException("Chỉ Admin mới có thể truy cập thông tin người dùng.");

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            if (!await IsCurrentUserAdminAsync())
                throw new UnauthorizedAccessException("Chỉ Admin mới có thể truy cập danh sách người dùng.");

            return await _userManager.Users.ToListAsync();
        }

        public async Task DeleteUserByIdAsync(string userId)
        {
            if (!await IsCurrentUserAdminAsync())
                throw new UnauthorizedAccessException("Chỉ Admin mới có thể xóa tài khoản.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task LockUserAsync(string userId)
        {
            if (!await IsCurrentUserAdminAsync())
                throw new UnauthorizedAccessException("Chỉ Admin mới có thể khóa tài khoản.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task UnlockUserAsync(string userId)
        {
            if (!await IsCurrentUserAdminAsync())
                throw new UnauthorizedAccessException("Chỉ Admin mới có thể mở tài khoản.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng.");
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                throw new Exception($"Vai trò '{roleName}' không tồn tại.");
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}

