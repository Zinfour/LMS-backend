namespace LMS.API.DTOs;

public class ModuleResourceDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedByUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? UpdatedByUserId { get; set; }
    public string? URL { get; set; }
    public string? ResourceType { get; set; }
    public int ModuleId { get; set; }
}