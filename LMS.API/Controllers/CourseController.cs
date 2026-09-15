using LMS.API.Core.Types;
using LMS.API.Core.Workflows;
using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.DTOs.Course;
using LMS.API.Models;
using LMS.API.Shell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CourseController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courseDtos = await _context.Course
                .Include(c => c.Modules).ThenInclude(m => m.Activities).ThenInclude(a => a.CompletedUsers)
                .Include(c => c.Modules).ThenInclude(m => m.Activities)
                .Include(c => c.Modules).ThenInclude(m => m.Resources)
                .Include(c => c.Users).ThenInclude(u => u.Roles)
                .Include(c => c.Resources)
                .Select(c => c.ToCourseDto()).ToListAsync();

            return Ok(courseDtos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("User not found.");
            var roles = await _userManager.GetRolesAsync(user);
            var caller = new CallerContext(user.Id,
                roles.Contains(Role.Teacher), roles.Contains(Role.Student), user.CourseId);

            var resolved = CourseAccess.ResolveCourseId(caller, id);
            if (resolved is null) return BadRequest("Invalid role.");

            var courseDto = await _context.Course
                .Where(c => c.Id == resolved.Value)
                .Include(c => c.Modules).ThenInclude(m => m.Activities).ThenInclude(a => a.CompletedUsers)
                .Include(c => c.Modules).ThenInclude(m => m.Activities)
                .Include(c => c.Modules).ThenInclude(m => m.Resources)
                .Include(c => c.Users).ThenInclude(u => u.Roles)
                .Include(c => c.Resources)
                .Select(c => c.ToCourseDto(user.Id)).FirstOrDefaultAsync();

            if (courseDto is null)
            {
                return NotFound("Course not found.");
            }

            return Ok(courseDto);
        }

        [HttpPost]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> CreateCourse(CreateCourseDto dto)
        {
            var now = DateTime.UtcNow;
            var course = new Course
            {
                CreatedAt = now,
                UpdatedAt = now,
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ImageURL = dto.ImageURL,
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
            if (course is null) return NotFound("Course not found.");

            course.UpdatedAt = DateTime.UtcNow;
            course.Name = dto.Name;
            course.Description = dto.Description;
            course.StartDate = dto.StartDate;
            course.EndDate = dto.EndDate;
            course.ImageURL = dto.ImageURL;

            await _context.SaveChangesAsync();
            return Ok(course);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course is null) return NotFound("Course not found.");
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}