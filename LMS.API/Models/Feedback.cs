namespace LMS.API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; } = null!;
        public string TeacherId { get; set; } = null!;
        public ApplicationUser Teacher { get; set; } = null!;
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; } = null!;
    }
}