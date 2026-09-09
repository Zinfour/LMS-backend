namespace LMS.API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Text { get; set; } = null!;
        public DateTime GivenAt { get; set; }
        public string TeacherId { get; set; } = null!;
        public ApplicationUser Teacher { get; set; } = null!;
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; } = null!;
    }
}