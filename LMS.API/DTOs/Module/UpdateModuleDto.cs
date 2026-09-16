using System.ComponentModel.DataAnnotations;

namespace LMS.API.DTOs;

public class UpdateModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    [Required]
    public DateOnly StartDate { get; set; }
    [Required]
    public DateOnly EndDate { get; set; }
    public string? ImageURL { get; set; }
}