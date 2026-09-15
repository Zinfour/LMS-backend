using LMS.API.Models;

namespace LMS.API.DTOs;

public class CreateActivityDto
{
    public ActivityType Type { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = null!;
    public string? ImageURL { get; set; }
    public int ModuleId { get; set; }
}