namespace LMS.API.DTOs
{
    public class CreateUserDto
    {
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? ImageUrl { get; set; }
        
        public int CourseId { get; set; }
    }
}