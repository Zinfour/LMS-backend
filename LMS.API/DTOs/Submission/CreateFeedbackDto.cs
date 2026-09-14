namespace LMS.API.DTOs;

public class CreateFeedbackDto
{
    public string Text { get; set; } = null!;
    public string TeacherId { get; set; } = null!;
}