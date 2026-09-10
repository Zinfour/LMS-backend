namespace LMS.API.DTOs;

public class CreateSubmissionDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Text { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public int AssignmentId { get; set; }
}
