using LMS.API.Models;

namespace LMS.API.DTOs.Course
{
    public class CreateCourseDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? ImageURL { get; set; }
    }
}
