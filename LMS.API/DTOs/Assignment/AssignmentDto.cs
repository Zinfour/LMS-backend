// namespace LMS.API.Models
// {
//     public class Assignment
//     {
//         public int Id { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public DateTime UpdatedAt { get; set; }

//         public string Title { get; set; } = null!;
//         public string Description { get; set; } = null!;
//         public DateTime Deadline { get; set; }
        
//         public int ActivityId { get; set; }
//         public Activity Activity { get; set; } = null!;
        
//         public List<Submission> Submissions { get; set; } = null!;
//     }
// }

namespace LMS.API.DTOs;

public class AssignmentDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Deadline { get; set; }
    public int ActivityId { get; set; }
    public List<SubmissionDto> Submissions {get; set;} = [];
}