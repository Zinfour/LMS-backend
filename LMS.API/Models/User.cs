namespace LMS.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; }
        public string? ImageUrl { get; set; }
        public List<Submission> Submissions { get; set; } = null!;
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}