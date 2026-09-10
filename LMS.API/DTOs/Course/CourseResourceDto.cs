using LMS.API.Models.Resources;

namespace LMS.API.DTOs.Course
{
    public class CourseResourceDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatedByUserId { get; set; } = null!;
        public string? UpdatedByUserId { get; set; }
        public string? URL { get; set; }
        public ResourceType ResourceType { get; set; }
    }
}
