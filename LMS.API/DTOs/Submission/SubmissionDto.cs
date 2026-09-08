// namespace LMS.API.Models
// {
//     public class Submission
//     {
//         public int Id { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public DateTime UpdatedAt { get; set; }

//         public string Text { get; set; } = null!;
//         public DateTime SubmittedAt { get; set; }

//         public string StudentId { get; set; } = null!;
//         public ApplicationUser Student { get; set; } = null!;

//         public int AssignmentId { get; set; }
//         public Assignment Assignment { get; set; } = null!;
//     }
// }

namespace LMS.API.DTOs;

public class SubmissionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public string StudentId { get; set; } = null!;
    public int AssignmentId { get; set; }
}