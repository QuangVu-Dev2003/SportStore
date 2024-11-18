using Microsoft.AspNetCore.Identity;

namespace SportStore.DataAccessLayer.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; } = null;
    }
}