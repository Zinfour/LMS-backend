namespace LMS.API.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public int CourseId { get; set; }
    }
}