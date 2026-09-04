namespace LMS.API.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ActivityType Type { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; } = null!;
        public List<ActivityResource> Resources { get; set; } = [];
        public string? ImageURL { get; set; }
        public Assignment? Assignment { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
    }
}