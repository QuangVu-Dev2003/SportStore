using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IUserService : IBaseService<AppUser>
    {
        // User
        Task<AppUser> GetCurrentUserAsync();
        //Task UpdateCurrentUserAsync(AppUser user);
        Task UpdateCurrentUserAsync(UpdateProfile updateProfile);
        Task MarkForDeletionAsync();
        Task RestoreAccountAsync();

        // Admin
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task DeleteUserByIdAsync(string userId);
        Task LockUserAsync(string userId);
        Task UnlockUserAsync(string userId);
        Task AssignRoleToUserAsync(string userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}