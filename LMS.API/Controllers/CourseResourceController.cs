using LMS.API.Data;
using LMS.API.DTOs.Resource;
using LMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
public class CourseResourceController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet("api/courses/{id}/resources")]
    [Authorize(Roles = Role.Teacher + "," + Role.Student)]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetCourseResources(int id)
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

        var resources = await _context.CourseResource.Where(r => r.CourseId == courseId).ToListAsync();
        var resourceDtos = resources.Select(r => new ResourceDto
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            CreatedByUserId = r.CreatedByUserId,
            UpdatedByUserId = r.UpdatedByUserId,
            URL = r.URL,
            ResourceType = Tools.ResourceTypeToString(r.ResourceType),
            Name = r.Name,
            Description = r.Description,
            CourseId = r.CourseId
        });

        return Ok(resourceDtos);
    }

    [HttpGet("api/courses/resources/{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<ResourceDto>> GetCourseResource(int id)
    {
        var resource = await _context.CourseResource.FindAsync(id);
        if (resource == null)
        {
            return NotFound($"Course resource with ID {id} not found.");
        }

        var resourceDto = new ResourceDto
        {
            Id = resource.Id,
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt,
            CreatedByUserId = resource.CreatedByUserId,
            UpdatedByUserId = resource.UpdatedByUserId,
            URL = resource.URL,
            ResourceType = Tools.ResourceTypeToString(resource.ResourceType),
            Name = resource.Name,
            Description = resource.Description,
            CourseId = resource.CourseId
        };

        return Ok(resourceDto);
    }

    [HttpPost("api/courses/{id}/resources")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<ResourceDto>> CreateCourseResource(int id, [FromBody] CreateResourceDto resourceDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("Logged-in User not found.");
        }

        var course = await _context.Course.FindAsync(id);
        if (course == null)
        {
            return NotFound($"Course with ID {id} not found.");
        }

        var resource = new CourseResource
        {
            CourseId = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            UpdatedByUserId = user.Id,
            Description = resourceDto.Description,
            Name = resourceDto.Name,
            URL = resourceDto.URL,
            ResourceType = Tools.ParseResourceType(resourceDto.ResourceType.ToString())
        };

        var result = new ResourceDto
        {
            Id = resource.Id,
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt,
            CreatedByUserId = resource.CreatedByUserId,
            UpdatedByUserId = resource.UpdatedByUserId,
            URL = resource.URL,
            ResourceType = Tools.ResourceTypeToString(resource.ResourceType),
            Name = resource.Name,
            Description = resource.Description,
            CourseId = resource.CourseId
        };

        _context.CourseResource.Add(resource);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourseResource), new { id = resource.Id }, resource);
    }

    [HttpPut("api/courses/resources/{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<ResourceDto>> UpdateCourseResource(int id, [FromBody] UpdateResourceDto resourceDto)
    {
        var resource = await _context.CourseResource.FindAsync(id);
        if (resource == null)
        {
            return NotFound($"Course resource with ID {id} not found.");
        }
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("Logged-in User not found.");
        }
        
        resource.UpdatedByUserId = user.Id;
        resource.UpdatedAt = DateTime.UtcNow;
        resource.Description = resourceDto.Description;
        resource.Name = resourceDto.Name;
        resource.URL = resourceDto.URL;
        resource.ResourceType = Tools.ParseResourceType(resourceDto.ResourceType.ToString());
        await _context.SaveChangesAsync();
        var result = new ResourceDto
        {
            Id = resource.Id,
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt,
            CreatedByUserId = resource.CreatedByUserId,
            UpdatedByUserId = resource.UpdatedByUserId,
            URL = resource.URL,
            ResourceType = Tools.ResourceTypeToString(resource.ResourceType),
            Name = resource.Name,
            Description = resource.Description,
            CourseId = resource.CourseId
        };

        return Ok(result);
    }

    [HttpDelete("api/courses/resources/{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<IActionResult> DeleteCourseResource(int id)
    {
        var resource = await _context.CourseResource.FindAsync(id);
        if (resource == null)
        {
            return NotFound($"Course resource with ID {id} not found.");
        }

        _context.CourseResource.Remove(resource);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}
