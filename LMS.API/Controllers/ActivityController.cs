using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.API.Controllers;

[ApiController]
[Route("/api/activities")]
public class ActivityController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities([FromQuery] int? courseId, [FromQuery] int? moduleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var isTeacher = User.IsInRole(Role.Teacher);
        var isStudent = User.IsInRole(Role.Student);

        if (!isTeacher && !isStudent)
        {
            return BadRequest("Invalid role.");
        }

        var activities = await _context.Activity
            .Where(a =>
                (courseId == null || a.Module.CourseId == courseId) &&
                (moduleId == null || a.ModuleId == moduleId) &&
                (isTeacher || a.Module.Course.Users.Any(u => u.Id == userId)))
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                Type = a.Type.ToString(),
                Name = a.Name,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Description = a.Description,
                ImageURL = a.ImageURL,
                ModuleId = a.ModuleId,

                Assignment = a.Assignment == null
                    ? null
                    : new AssignmentDto
                    {
                        Id = a.Assignment.Id,
                        CreatedAt = a.Assignment.CreatedAt,
                        UpdatedAt = a.Assignment.UpdatedAt,
                        Title = a.Assignment.Title,
                        Description = a.Assignment.Description,
                        Deadline = a.Assignment.Deadline,
                        ActivityId = a.Assignment.ActivityId,
                        Submissions = a.Assignment.Submissions
                            .Select(s => new SubmissionDto
                            {
                                Id = s.Id,
                                CreatedAt = s.CreatedAt,
                                Text = s.Text,
                                StudentId = s.StudentId,
                                AssignmentId = s.AssignmentId
                            })
                            .ToList()
                    },

                Resources = a.Resources
                    .Select(r => new ActivityResourceDto
                    {
                        Id = r.Id,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        CreatedByUserId = r.CreatedByUserId,
                        UpdatedByUserId = r.UpdatedByUserId,
                        URL = r.URL,
                        ResourceType = r.ResourceType.ToString(),
                        ActivityId = r.ActivityId
                    })
                    .ToList()
            })
            .ToListAsync();

        return activities;
    }

    [HttpGet("{activityId}")]
    [Authorize]
    public async Task<ActionResult<ActivityDto>> GetActivity(int activityId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var isTeacher = User.IsInRole(Role.Teacher);
        var isStudent = User.IsInRole(Role.Student);

        if (!isTeacher && !isStudent)
        {
            return BadRequest("Invalid role.");
        }

        var activity = await _context.Activity
            .Where(a =>
                a.Id == activityId &&
                (isTeacher || a.Module.Course.Users.Any(u =>
                    u.Id == userId)))
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                Type = a.Type.ToString(),
                Name = a.Name,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Description = a.Description,
                ImageURL = a.ImageURL,
                ModuleId = a.ModuleId,

                Assignment = a.Assignment == null
                    ? null
                    : new AssignmentDto
                    {
                        Id = a.Assignment.Id,
                        CreatedAt = a.Assignment.CreatedAt,
                        UpdatedAt = a.Assignment.UpdatedAt,
                        Title = a.Assignment.Title,
                        Description = a.Assignment.Description,
                        Deadline = a.Assignment.Deadline,
                        ActivityId = a.Assignment.ActivityId,

                        Submissions = a.Assignment.Submissions
                            .Select(s => new SubmissionDto
                            {
                                Id = s.Id,
                                CreatedAt = s.CreatedAt,
                                Text = s.Text,
                                StudentId = s.StudentId,
                                AssignmentId = s.AssignmentId
                            })
                            .ToList()
                    },

                Resources = a.Resources
                    .Select(r => new ActivityResourceDto
                    {
                        Id = r.Id,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        CreatedByUserId = r.CreatedByUserId,
                        UpdatedByUserId = r.UpdatedByUserId,
                        URL = r.URL,
                        ResourceType = r.ResourceType.ToString(),
                        ActivityId = r.ActivityId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (activity == null)
        {
            return NotFound($"Couldn't find activity with id: {activityId}.");
        }

        return activity;
    }

    [HttpPost]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> CreateActivity()
    {
        return BadRequest();
    }

    [HttpPut]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> UpdateActivity()
    {
        return BadRequest();
    }

    [HttpDelete]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> DeleteActivity()
    {
        return BadRequest();
    }

    [HttpPost("{activityId}/complete/{userId}")]
    [Authorize]
    public async Task<ActionResult> CompleteActivity(int activityId, string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        if (!User.IsInRole(Role.Teacher) && currentUserId != userId)
        {
            return Forbid();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound($"Couldn't find user with id: {userId}.");
        }

        var activity = await _context.Activity.FindAsync(activityId);

        if (activity == null)
        {
            return NotFound($"Couldn't find activity with id: {activityId}.");
        }

        var alreadyCompleted = await _context.Activity
            .Where(a => a.Id == activityId)
            .SelectMany(a => a.CompletedUsers)
            .AnyAsync(u => u.Id == userId);

        if (alreadyCompleted)
        {
            return NoContent();
        }

        activity.CompletedUsers.Add(user);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}