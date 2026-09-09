using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.DTOs.Course;
using LMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CourseController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        return await _context.Course
            .Select(c => new CourseDto
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Name = c.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                ImageURL = c.ImageURL,
                Resources = c.Resources.Select(r => new CourseResourceDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    CreatedByUserId = r.CreatedByUserId,
                    UpdatedByUserId = r.UpdatedByUserId,
                    URL = r.URL,
                    ResourceType = r.ResourceType
                }).ToList(),
                Users = c.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                }).ToList(),
            }).ToListAsync();
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
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

        var course = await _context.Course
            .Where(c => c.Id == courseId)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Name = c.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                ImageURL = c.ImageURL,
                Resources = c.Resources.Select(r => new CourseResourceDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    CreatedByUserId = r.CreatedByUserId,
                    UpdatedByUserId = r.UpdatedByUserId,
                    URL = r.URL,
                    ResourceType = r.ResourceType
                }).ToList(),
                Users = roles.Contains(Role.Teacher) ? c.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                }).ToList() : new List<UserDto>(),
            })
            .FirstOrDefaultAsync();

        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    [HttpPost]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> CreateCourse(CreateCourseDto dto)
    {
        var course = new Course
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ImageURL = dto.ImageURL,
            Resources = dto.Resources.Select(r => new CourseResource
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                CreatedByUserId = r.CreatedByUserId,
                UpdatedByUserId = r.UpdatedByUserId,
                URL = r.URL,
                ResourceType = r.ResourceType
            }).ToList() 
            ?? new List<CourseResource>(),
        };

        _context.Course.Add(course);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> UpdateCourse(int id, UpdateCourseDto dto)
    {
        var course = await _context.Course.FindAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        course.UpdatedAt = DateTime.UtcNow;
        course.Name = dto.Name;
        course.Description = dto.Description;
        course.StartDate = dto.StartDate;
        course.EndDate = dto.EndDate;
        course.ImageURL = dto.ImageURL;
        course.Resources = dto.Resources?.Select(r => new CourseResource
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            CreatedByUserId = r.CreatedByUserId,
            UpdatedByUserId = r.UpdatedByUserId,
            URL = r.URL,
            ResourceType = r.ResourceType
        }).ToList()
        ?? new List<CourseResource>();

        _context.Course.Update(course);
        await _context.SaveChangesAsync();

        return Ok(course);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult> DeleteCourse(int id)
    {
        var course = await _context.Course.FindAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        _context.Course.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
