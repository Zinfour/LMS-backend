using LMS.API.Core.Types;
using LMS.API.Core.Workflows;
using LMS.API.Data;
using LMS.API.DTOs.Resource;
using LMS.API.Models;
using LMS.API.Shell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    public class CourseResourceController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet("api/courses/{id}/resources")]
        [Authorize(Roles = Role.Teacher + "," + Role.Student)]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetCourseResources(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var caller = new CallerContext(user.Id,
                roles.Contains(Role.Teacher), roles.Contains(Role.Student), user.CourseId);

            // Core: pure decision about which course to look at.
            var resolved = CourseAccess.ResolveCourseId(caller, id);
            if (resolved is null) return BadRequest("Invalid role.");

            // Shell: query.
            var resources = await _context.CourseResource
                .Where(r => r.CourseId == resolved)
                .ToListAsync();

            return Ok(resources.Select(r => r.ToResourceDto()));
        }

        [HttpGet("api/courses/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> GetCourseResource(int id)
        {
            var resource = await _context.CourseResource.FindAsync(id);
            return resource is null
                ? NotFound($"Course resource with ID {id} not found.")
                : Ok(resource.ToResourceDto());
        }

        [HttpPost("api/courses/{id}/resources")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> CreateCourseResource(
            int id, [FromBody] CreateResourceDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("Logged-in User not found.");

            var caller = new CallerContext(user.Id, IsTeacher: true, IsStudent: false, user.CourseId);
            var courseExists = await _context.Course.AnyAsync(c => c.Id == id);

            var validation = ResourceWorkflow.ValidateForCreate(new ResourceWriteContext(
                Caller: caller,
                ParentExists: courseExists,
                ParentId: id,
                Write: new ResourceWrite(dto.URL, dto.ResourceType, dto.Name, dto.Description)));

            if (validation is not WorkflowResult<ResourceWrite>.Ok ok)
                return this.ToActionResult(validation);

            var now = DateTime.UtcNow;
            var resource = new CourseResource
            {
                CourseId = id,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedByUserId = user.Id,
                UpdatedByUserId = user.Id,
                Description = ok.Value.Description,
                Name = ok.Value.Name,
                URL = ok.Value.URL,
                ResourceType = Tools.ParseResourceType(ok.Value.ResourceType)
            };

            _context.CourseResource.Add(resource);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseResource),
                new { id = resource.Id }, resource.ToResourceDto());
        }

        [HttpPut("api/courses/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> UpdateCourseResource(
            int id, [FromBody] UpdateResourceDto dto)
        {
            var resource = await _context.CourseResource.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("Logged-in User not found.");

            var caller = new CallerContext(user.Id, IsTeacher: true, IsStudent: false, user.CourseId);

            var validation = ResourceWorkflow.ValidateForUpdate(new ResourceWriteContext(
                Caller: caller,
                ParentExists: resource is not null,
                ParentId: id,
                Write: new ResourceWrite(dto.URL, dto.ResourceType, dto.Name, dto.Description)));

            if (validation is not WorkflowResult<ResourceWrite>.Ok ok)
                return this.ToActionResult(validation);

            // Shell: apply.
            resource!.UpdatedByUserId = user.Id;
            resource.UpdatedAt = DateTime.UtcNow;
            resource.Description = ok.Value.Description;
            resource.Name = ok.Value.Name;
            resource.URL = ok.Value.URL;
            resource.ResourceType = Tools.ParseResourceType(ok.Value.ResourceType);
            await _context.SaveChangesAsync();

            return Ok(resource.ToResourceDto());
        }

        [HttpDelete("api/courses/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<IActionResult> DeleteCourseResource(int id)
        {
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();

            var resource = await _context.CourseResource.FindAsync(id);

            var decision = ResourceWorkflow.ValidateForDelete(caller, resource is not null, id);
            if (decision is not WorkflowResult<Unit>.Ok)
                return this.ToActionResult(decision);

            _context.CourseResource.Remove(resource!);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}