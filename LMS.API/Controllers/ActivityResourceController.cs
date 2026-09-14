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
    public class ActivityResourceController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet("api/activities/{id}/resources")]
        [Authorize(Roles = Role.Teacher + "," + Role.Student)]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetActivityResources(int id)
        {
            if (!await _context.Activity.AnyAsync(a => a.Id == id))
                return NotFound($"Activity with ID {id} not found.");

            var dtos = await _context.ActivityResource
                .Where(r => r.ActivityId == id)
                .Select(r => r.ToResourceView())    // pure projection helper
                .ToListAsync();

            return Ok(dtos.Select(v => v.ToDto()));
        }

        [HttpGet("api/activities/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> GetActivityResource(int id)
        {
            var view = await _context.ActivityResource
                .Where(r => r.Id == id)
                .Select(r => r.ToResourceView())
                .FirstOrDefaultAsync();

            return view is null
                ? NotFound($"Resource with ID {id} not found.")
                : Ok(view.ToDto());
        }

        [HttpPost("api/activities/{id}/resources")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> CreateActivityResource(
            int id, [FromBody] CreateResourceDto dto)
        {
            // 1. Shell: resolve caller + existence facts.
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized("User not found.");

            if (!await _context.Activity.AnyAsync(a => a.Id == id))
                return NotFound($"Activity with ID {id} not found.");

            // 2. Core: validate the request purely.
            var validation = ResourceWorkflow.Validate(new ResourceWrite(
                dto.URL, dto.ResourceType, dto.Name, dto.Description));

            if (validation is not WorkflowResult<ResourceWrite>.Ok ok)
                return this.ToActionResult(validation);

            // 3. Shell: persist.
            var now = DateTime.UtcNow;
            var resource = new ActivityResource
            {
                ActivityId = id,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedByUserId = user.Id,
                UpdatedByUserId = user.Id,
                URL = ok.Value.URL,
                ResourceType = Tools.ParseResourceType(ok.Value.ResourceType),
                Name = ok.Value.Name,
                Description = ok.Value.Description
            };
            _context.ActivityResource.Add(resource);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivityResource),
                new { id = resource.Id }, resource.ToResourceView().ToDto());
        }

        [HttpPut("api/activities/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> UpdateActivityResource(
            int id, [FromBody] UpdateResourceDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized("User not found.");

            var resource = await _context.ActivityResource.FindAsync(id);
            if (resource is null) return NotFound($"Resource with ID {id} not found.");

            var validation = ResourceWorkflow.Validate(new ResourceWrite(
                dto.URL, dto.ResourceType, dto.Name, dto.Description));
            if (validation is not WorkflowResult<ResourceWrite>.Ok ok)
                return this.ToActionResult(validation);

            resource.UpdatedAt = DateTime.UtcNow;
            resource.UpdatedByUserId = user.Id;
            resource.URL = ok.Value.URL;
            resource.ResourceType = Tools.ParseResourceType(ok.Value.ResourceType);
            resource.Name = ok.Value.Name;
            resource.Description = ok.Value.Description;

            await _context.SaveChangesAsync();
            return Ok(resource.ToResourceView().ToDto());
        }

        [HttpDelete("api/activities/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> DeleteActivityResource(int id)
        {
            var resource = await _context.ActivityResource.FindAsync(id);
            if (resource is null) return NotFound($"Resource with ID {id} not found.");
            _context.ActivityResource.Remove(resource);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}