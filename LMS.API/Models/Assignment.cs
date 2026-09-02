namespace LMS.API.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public int ActivityId { get; set; }
        public List<Submission> Submissions { get; set; } = null!;
    }
}