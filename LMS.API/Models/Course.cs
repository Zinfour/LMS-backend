using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.API.Models
{
    public class Course
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<Module> Modules { get; set; } = [];
        public string? ImageURL { get; set; }
        public List<CourseResource> Resources { get; set; } = [];
        public List<ApplicationUser> Users { get; set; } = [];
    }
}