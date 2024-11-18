using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IUserService : IBaseService<AppUser>
    {
        // Normal User
        Task<AppUser> GetCurrentUserAsync();
        Task UpdateCurrentUserAsync(AppUser user);
        Task MarkForDeletionAsync();
        Task RestoreAccountAsync();

        // Admin
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task DeleteUserByIdAsync(string userId);
        Task LockUserAsync(string userId);
        Task UnlockUserAsync(string userId);
        Task AssignRoleToUserAsync(string userId, string roleName);
    }
}
