using Microsoft.AspNetCore.Identity;

namespace SportStore.DataAccessLayer.Models
{
    public class AppRole : IdentityRole
    {
        public string Description { get; set; }
    }
}