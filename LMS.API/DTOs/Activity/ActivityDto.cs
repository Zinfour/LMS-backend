// namespace LMS.API.Models
// {
//     public class Activity
//     {
//         public int Id { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public DateTime UpdatedAt { get; set; }

//         public ActivityType Type { get; set; }
//         public string Name { get; set; } = null!;
//         public DateTime StartTime { get; set; }
//         public DateTime EndTime { get; set; }
//         public string Description { get; set; } = null!;
//         public List<ActivityResource> Resources { get; set; } = [];
//         public string? ImageURL { get; set; }
//         public Assignment? Assignment { get; set; }
//         public int ModuleId { get; set; }
//         public Module Module { get; set; } = null!;
//     }
// }

// ActivityType.cs
// namespace LMS.API.Models
// {
//     public enum ActivityType
//     {
//         Seminar,
//         ELearning,
//         Practice,
//         Assignment,
//         Other,
//     }
// }

namespace LMS.API.DTOs;

public class ActivityDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Type {get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = null!;
    public string? ImageURL { get; set; }
    public int ModuleId { get; set; }
		public bool Completed { get; set; }
    public AssignmentDto? Assignment {get; set;}
    public List<ActivityResourceDto> Resources {get; set;} = []; 

}