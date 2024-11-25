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
        public int? Phone { get; set; }
        public string? Address { get; set; }
        public string AccountType { get; set; } = "Thường";
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
    }
}