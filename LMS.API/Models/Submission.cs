namespace LMS.API.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Text { get; set; } = null!;
        public DateTime SubmittedAt { get; set; }
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
    }
}