using LMS.API.Models.Resources;

namespace LMS.API.Models
{
    public class CourseResource
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int CreatedByUserId { get; set; }
        public int? UpdatedByUserId  { get; set; }
        public string? URL { get; set; }
        public ResourceType ResourceType { get; set; }
    }
}