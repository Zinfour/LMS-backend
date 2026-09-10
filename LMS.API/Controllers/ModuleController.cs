using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace LMS.API.Controllers;

[ApiController]
[Route("/api/courses/{courseId}/modules")]
public class ModuleController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules(int courseId)
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

        return await _context.Module
            .Where(m => m.CourseId == id)
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                ImageURL = m.ImageURL,
                CourseId = m.CourseId,
                ActivitiesNumber = m.Activities.Count,
                ResourcesNumber = m.Resources.Count,
                NumberOfCompletedActivities = m.Activities.Where(a => a.CompletedUsers.Any(u => u.Id == user.Id)).ToList().Count,
                Order = m.Order,
                CurrentStatus = Tools.calculateStatus(m, user)
            }).ToListAsync();
    }


    [HttpGet("{moduleId}")]
    public async Task<ActionResult<ModuleFullDto>> GetModule(int courseId, int moduleId)
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


        var module = await _context.Module.Where(m => m.CourseId == id && m.Id == moduleId).FirstOrDefaultAsync();

        if(module == null)
        {
            return BadRequest("Invalid ModuleId");
        }

        var activities = module.Activities.Select(a =>
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

        var resources = module.Resources.Select(r => new ModuleResourceDto
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                CreatedByUserId = r.CreatedByUserId,
                UpdatedByUserId = r.UpdatedByUserId,
                URL = r.URL,
                ResourceType = r.ResourceType.ToString(),
                ModuleId = r.ModuleId
            }).ToList();

        return new ModuleFullDto
        {
            Id = module.Id,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt,
            Name = module.Name,
            Description = module.Description,
            StartDate = module.StartDate,
            EndDate = module.EndDate,
            ImageURL = module.ImageURL,
            Activities = activities,
            Resources = resources,
            CourseId = module.CourseId,
            TotalNumberOfModules = _context.Course.FirstOrDefault(c => c.Id == module.CourseId)!.Modules.Count,
            Order = module.Order
        };

    }

    [HttpPost]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> createModule(int id)
    {
        return BadRequest();
    }

    [HttpPut("{moduleId}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> updateModule(int id, int moduleId)
    {
        return BadRequest();
    }

    [HttpDelete("{moduleId}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> deleteModule(int id, int moduleId)
    {
        return BadRequest();
    }
}