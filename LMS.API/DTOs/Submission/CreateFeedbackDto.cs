namespace LMS.API.DTOs;

public class CreateFeedbackDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Text { get; set; } = null!;
    public DateTime GivenAt { get; set; }
    public string TeacherId { get; set; } = null!;
    public UserDto Teacher { get; set; } = null!;
}