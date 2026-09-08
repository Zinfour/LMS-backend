// using LMS.API.Models.Resources;

// namespace LMS.API.Models
// {
//     public class ModuleResource
//     {
//         public int Id { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public DateTime UpdatedAt { get; set; }

//         public string CreatedByUserId { get; set; } = null!;
//         public string? UpdatedByUserId { get; set; }
//         public string? URL { get; set; }
//         public ResourceType ResourceType { get; set; }

//         public int ModuleId { get; set; }
//         public Module Module { get; set; } = null!;
//     }
// }

// namespace LMS.API.Models.Resources
// {
//     public enum ResourceType
//     {
//         Instruction,
//         TextMaterial,
//         Link,
//         Summary, 
//         Reference,
//     }
// }

namespace LMS.API.DTOs;

public class ModuleResourceDto
{
    public int Id { get; set; }
    public string CreatedByUserId { get; set; } = null!;
    public string? UpdatedByUserId { get; set; }
    public string? URL { get; set; }
    public string? ResourceType { get; set; }

    public int ModuleId { get; set; }
}