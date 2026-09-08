// Teachers should be able to create, read, update and delete modules for a course.
// Only users that are logged in and have the role Teacher should be able to perform these operations.

// When creating or updating a module make sure that the start and end times are correct (start time before end time) and that they do not overlap the start or end time of other modules for the same course.
// Also make sure that the times are within the start and end time of the course.

// GET
// POST
// PUT
// DELETE
// /api/courses/{id}/modules


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
[Route("/api/courses/{id}/modules")]
public class ModuleController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> getModules(int id)
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

        var courseId = roles.Contains(Role.Teacher) ? id : user.CourseId;
        
        
        var courseExists = await _context.Course.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
        {
            return BadRequest("Invalid CourseId.");
        }

        return await _context.Module
            .Where(m => m.CourseId == courseId)
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                ImageURL = m.ImageURL,
                CourseId = m.CourseId
            }).ToListAsync();
    }

    [HttpGet("{moduleId}")]
    public async Task<ActionResult<ModuleDto>> getModule(int id, int moduleId)
    {
        return BadRequest();
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

// Amers getModules
// [HttpGet("{id}/modules")]
//     [Authorize]
//     public async Task<ActionResult<IEnumerable<CourseModuleDto>>> GetCourseModules(int id)
//     {

//         var user = await _userManager.GetUserAsync(User);

//         if (user == null)
//         {
//             return BadRequest("User not found.");
//         }

//         var roles = await _userManager.GetRolesAsync(user);

//         if (!roles.Contains(Role.Teacher) && !roles.Contains(Role.Student))
//         {
//             return BadRequest($"Invalid role.");
//         }

//         var courseId = roles.Contains(Role.Teacher) ? id : user.CourseId;

//         var course = await _context.Course.Where(c => c.Id == courseId)
//             .FirstOrDefaultAsync();

//         if (course == null)
//         {
//             return NotFound("Course not found.");
//         }

//         var modules = await _context.Module
//             .Where(m => m.CourseId == courseId)
//             .Select(m => new CourseModuleDto
//             {
//                 Id = m.Id,
//                 CreatedAt = m.CreatedAt,
//                 UpdatedAt = m.UpdatedAt,
//                 Name = m.Name,
//                 Description = m.Description,
//                 StartDate = m.StartDate,
//                 EndDate = m.EndDate,
//                 Resources = m.Resources.Select(r => new CourseResourceDto
//                 {
//                     Id = r.Id,
//                     CreatedAt = r.CreatedAt,
//                     UpdatedAt = r.UpdatedAt,
//                     CreatedByUserId = r.CreatedByUserId,
//                     UpdatedByUserId = r.UpdatedByUserId,
//                     URL = r.URL,
//                     ResourceType = r.ResourceType
//                 }).ToList()
//             }).ToListAsync();

//         return Ok(modules);
//     }