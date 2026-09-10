namespace LMS.API.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Text { get; set; } = null!;
        public bool Overdue { get; set; }

        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public LinkedList<Feedback> Feedbacks { get; set; } = new LinkedList<Feedback>();
    }
}