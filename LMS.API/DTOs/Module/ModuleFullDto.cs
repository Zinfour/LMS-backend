// namespace LMS.API.Models
// {
//     public class Module
//     {
//         public int Id { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public DateTime UpdatedAt { get; set; }

//         public string Name { get; set; } = null!;
//         public string Description { get; set; } = null!;
//         public DateOnly StartDate { get; set; }
//         public DateOnly EndDate { get; set; }
//         public string? ImageURL { get; set; }
//         public List<Activity> Activities { get; set; } = [];
//         public List<ModuleResource> Resources { get; set; } = [];
        
//         public int CourseId { get; set; }
//         public Course Course { get; set; } = null!;
//     }
// }

namespace LMS.API.DTOs;

public class ModuleFullDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? ImageURL { get; set; }
    public List<ActivityDto> Activities { get; set; } = [];
    public List<ModuleResourceDto> Resources { get; set; } = [];
    
    public int CourseId { get; set; }
}