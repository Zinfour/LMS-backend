namespace LMS.API.DTOs;

public class SubmissionDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Text { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public string StudentId { get; set; } = null!;
    public int AssignmentId { get; set; }

    public List<FeedbackDto> Feedbacks { get; set; } = new List<FeedbackDto>();
}
