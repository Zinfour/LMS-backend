using Microsoft.AspNetCore.Identity;

namespace LMS.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public List<Submission> Submissions { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public ICollection<ApplicationRole> Roles { get; set; } = [];
    }
}
