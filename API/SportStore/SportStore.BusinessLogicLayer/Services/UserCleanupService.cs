using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services
{
    public class UserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UserCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    var usersToDelete = await userManager.Users
                        .Where(u => u.IsDeleted && u.DeletionDate.HasValue && u.DeletionDate.Value.AddDays(30) <= DateTime.UtcNow)
                        .ToListAsync();

                    foreach (var user in usersToDelete)
                    {
                        await userManager.DeleteAsync(user);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Kiểm tra hàng ngày
            }
        }
    }
}