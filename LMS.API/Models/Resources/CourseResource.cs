using LMS.API.Models.Resources;

namespace LMS.API.Models
{
    public class CourseResource
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatedByUserId { get; set; } = null!;
        public string? UpdatedByUserId { get; set; }
        public string? URL { get; set; }
        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}