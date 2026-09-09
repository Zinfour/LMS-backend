using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
[Route("/api/course/{courseId}/modules/{moduleId}/activities")]
public class ActivityController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;




    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities(int courseId, int moduleId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (!roles.Contains(Role.Teacher) && !roles.Contains(Role.Student))
        {
            return BadRequest($"Invalid role.");
        }

        var id = roles.Contains(Role.Teacher) ? courseId : user.CourseId;
        
        
        var courseExists = await _context.Course.AnyAsync(c => c.Id == id);
        if (!courseExists)
        {
            return BadRequest("Invalid CourseId.");
        }

        var module = await _context.Module.Where(m => m.Id == moduleId).FirstOrDefaultAsync();

        if(module == null)
        {
            return BadRequest("Invalid ModuleId");
        }

        return module.Activities.Select(a =>
        {
            var tempAssignment = a.Assignment;
            var assignment = tempAssignment == null ? null : new AssignmentDto
                {
                    Id = tempAssignment.Id,
                    CreatedAt = tempAssignment.CreatedAt,
                    UpdatedAt = tempAssignment.UpdatedAt,
                    Title = tempAssignment.Title,
                    Description = tempAssignment.Description,
                    Deadline = tempAssignment.Deadline,
                    ActivityId = tempAssignment.ActivityId,
                    Submissions = tempAssignment.Submissions.Select(s => new SubmissionDto
                    {
                        Id = s.Id,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt,
                        SubmittedAt = s.SubmittedAt,
                        Text = s.Text,
                        StudentId = s.StudentId,
                        AssignmentId = s.AssignmentId
                    }).ToList()
                };
            var resources = a.Resources.Select(r => new ActivityResourceDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    CreatedByUserId = r.CreatedByUserId,
                    UpdatedByUserId = r.UpdatedByUserId,
                    URL = r.URL,
                    ResourceType = r.ResourceType.ToString(),
                    ActivityId = r.ActivityId
                }).ToList();
            return new ActivityDto
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
                Assignment = assignment,
                Resources = resources
            };
        }).ToList();
    }
}