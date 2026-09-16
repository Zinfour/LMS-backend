namespace LMS.API.DTOs;

public class CreateAssignmentDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Deadline { get; set; }
    public int ActivityId { get; set; }
}