namespace LMS.API.DTOs;

public class SubmissionDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Text { get; set; } = null!;
    public bool Overdue { get; set; }
    public string StudentId { get; set; } = null!;
    public int AssignmentId { get; set; }

    public List<FeedbackDto> Feedback { get; set; } = new List<FeedbackDto>();
}
