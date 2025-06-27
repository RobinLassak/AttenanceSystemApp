using Microsoft.AspNetCore.Identity;

namespace AttenanceSystemApp.Models
{
    public class AppUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
