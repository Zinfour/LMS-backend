using LMS.API.Models;

namespace LMS.API.DTOs.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? ImageURL { get; set; }
        public List<CourseResourceDto> Resources { get; set; } = [];
				public List<ModuleDto> Modules { get; set; } = [];
        public List<UserDto> Students { get; set; } = [];

				public UserDto Teacher { get; set; } = null!;
    }
}
