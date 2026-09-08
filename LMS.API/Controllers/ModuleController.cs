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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace LMS.API.Controllers;

[ApiController]
[Route("/api/courses/{courseId}/modules")]
public class ModuleController(LmsContext lmsContext) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> getModules(int courseId)
    {
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
    public async Task<ActionResult<ModuleDto>> getModule(int courseId, int moduleId)
    {
        return BadRequest();
    }

    [HttpPost]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> createModule(int courseId)
    {
        return BadRequest();
    }

    [HttpPut("{moduleId}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> updateModule(int courseId, int moduleId)
    {
        return BadRequest();
    }

    [HttpDelete("{moduleId}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> deleteModule(int courseId, int moduleId)
    {
        return BadRequest();
    }
}