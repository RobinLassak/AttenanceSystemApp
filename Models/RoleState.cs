using Microsoft.AspNetCore.Identity;

namespace AttenanceSystemApp.Models
{
    public class RoleState
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
}
