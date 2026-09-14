using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

/// <summary>
/// Controller for handling submission-related operations.
/// </summary>
[ApiController]
public class SubmissionController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet("api/users/{id}/submissions")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetSubmissions(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("Logged-in User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Role.Teacher) && !roles.Contains(Role.Student))
        {
            return BadRequest("Invalid role.");
        }

        var userId = roles.Contains(Role.Teacher) ? id : user.Id;


        var selectedUser = await _context.Users
            .Include(u => u.Submissions)
                .ThenInclude(s => s.Assignment)
            .Include(u => u.Submissions)
                .ThenInclude(s => s.Feedbacks)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (selectedUser == null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        var submissions = selectedUser.Submissions.Select(s => new SubmissionDto
        {
            Id = s.Id,
            CreatedAt = s.CreatedAt,
            Text = s.Text,
            StudentId = s.StudentId,
            AssignmentId = s.AssignmentId,
            Overdue = s.Assignment != null && s.CreatedAt > s.Assignment.Deadline,
            Feedback = s.Feedbacks.Select(f => new FeedbackDto
            {
                Id = f.Id,
                CreatedAt = f.CreatedAt,
                Text = f.Text,
                TeacherId = f.TeacherId
            }).ToList()
        }).ToList();

        return Ok(submissions);
    }

    [HttpPost("api/submissions")]
    [Authorize(Roles = Role.Student)]
    public async Task<ActionResult<SubmissionDto>> CreateSubmission([FromBody] CreateSubmissionDto createSubmissionDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("Logged-in User not found.");
        }

        var assignment = await _context.Assignment.FindAsync(createSubmissionDto.AssignmentId);
        if (assignment == null)
        {
            return NotFound($"Assignment with ID {createSubmissionDto.AssignmentId} not found.");
        }

        var submission = new Submission
        {
            Text = createSubmissionDto.Text,
            CreatedAt = DateTime.UtcNow,
            StudentId = user.Id,
            AssignmentId = createSubmissionDto.AssignmentId,
            Overdue = DateTime.UtcNow > assignment.Deadline
        };

        _context.Submission.Add(submission);
        await _context.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            Id = submission.Id,
            CreatedAt = submission.CreatedAt,
            Text = submission.Text,
            Overdue = submission.Overdue,
            StudentId = submission.StudentId,
            AssignmentId = submission.AssignmentId,
            Feedback = [],
        };
        return CreatedAtAction(nameof(GetSubmissions), new { id = user.Id }, submissionDto);
    }

    [HttpGet("api/submissions/{id}/feedback")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedback(int id)
    {
        var submission = await _context.Submission.FindAsync(id);
        if (submission == null)
        {
            return NotFound($"Submission with ID {id} not found.");
        }

        var feedbacks = await _context.Feedback.Where(f => f.SubmissionId == id).ToListAsync();

        return Ok(feedbacks.Select(f => new FeedbackDto
        {
            Id = f.Id,
            CreatedAt = f.CreatedAt,
            Text = f.Text,
            TeacherId = f.TeacherId
        }));
    }

    [HttpPost("api/submissions/{id}/feedback")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<FeedbackDto>> AddFeedback(int id, [FromBody] CreateFeedbackDto createFeedbackDto)
    {
        var submission = await _context.Submission.FindAsync(id);
        if (submission == null)
        {
            return NotFound($"Submission with ID {id} not found.");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("Logged-in User not found.");
        }

        var feedback = new Feedback
        {
            CreatedAt = DateTime.UtcNow,
            Text = createFeedbackDto.Text,
            TeacherId = user.Id,
            SubmissionId = id
        };

        _context.Feedback.Add(feedback);
        await _context.SaveChangesAsync();

        var feedbackDto = new FeedbackDto
        {
            Id = feedback.Id,
            CreatedAt = feedback.CreatedAt,
            Text = feedback.Text,
            TeacherId = feedback.TeacherId,
        };

        return CreatedAtAction(nameof(GetFeedback), new { id = submission.Id }, feedbackDto);
    }

    [HttpDelete("api/feedback/{feedbackId}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> DeleteFeedback(int feedbackId)
    {
        var feedback = await _context.Feedback.FindAsync(feedbackId);
        if (feedback == null)
        {
            return NotFound($"Feedback with ID {feedbackId} not found.");
        }

        _context.Feedback.Remove(feedback);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
