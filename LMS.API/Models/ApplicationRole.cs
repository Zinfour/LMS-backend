using Microsoft.AspNetCore.Identity;

namespace LMS.API.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

        public ICollection<ApplicationUser> Users { get; set; } = [];
    }
}
