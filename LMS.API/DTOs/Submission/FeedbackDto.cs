namespace LMS.API.DTOs;

public class FeedbackDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Text { get; set; } = null!;
    public string TeacherId { get; set; } = null!;
    public UserDto Teacher { get; set; } = null!;
}