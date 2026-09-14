using System.ComponentModel.DataAnnotations;

namespace LMS.API.DTOs;

public class ModuleForCreatingDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    [Required]
    public DateOnly StartDate { get; set; }
    [Required]
    public DateOnly EndDate { get; set; }
    public string? ImageURL { get; set; }
    // public int CourseId { get; set; }
}