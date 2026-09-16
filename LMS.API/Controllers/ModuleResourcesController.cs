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
    public class ModuleResourcesController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet("api/modules/{id}/resources")]
        [Authorize(Roles = Role.Teacher + "," + Role.Student)]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetModuleResources(int id)
        {
            var resources = await _context.ModuleResource
                .Where(r => r.ModuleId == id)
                .ToListAsync();

            return Ok(resources.Select(r => r.ToResourceDto()));
        }

        [HttpGet("api/modules/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> GetModuleResource(int id)
        {
            var resource = await _context.ModuleResource.FindAsync(id);
            return resource is null
                ? NotFound($"Module resource with ID {id} not found.")
                : Ok(resource.ToResourceDto());
        }

        [HttpPost("api/modules/{id}/resources")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> CreateModuleResource(
            int id, [FromBody] CreateResourceDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("Logged-in User not found.");

            var caller = new CallerContext(user.Id, IsTeacher: true, IsStudent: false, user.CourseId);
            var moduleExists = await _context.Module.AnyAsync(m => m.Id == id);

            var validation = ResourceWorkflow.ValidateForCreate(new ResourceWriteContext(
                Caller: caller,
                ParentExists: moduleExists,
                ParentId: id,
                Write: new ResourceWrite(dto.URL, dto.ResourceType, dto.Name, dto.Description)));

            if (validation is not WorkflowResult<ResourceWrite>.Ok ok)
                return this.ToActionResult(validation);

            var now = DateTime.UtcNow;
            var resource = new ModuleResource
            {
                ModuleId = id,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedByUserId = user.Id,
                UpdatedByUserId = user.Id,
                Description = ok.Value.Description,
                Name = ok.Value.Name,
                URL = ok.Value.URL,
                ResourceType = Tools.ParseResourceType(ok.Value.ResourceType)
            };

            _context.ModuleResource.Add(resource);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModuleResource),
                new { id = resource.Id }, resource.ToResourceDto());
        }

        [HttpPut("api/modules/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<ResourceDto>> UpdateModuleResource(
            int id, [FromBody] UpdateResourceDto dto)
        {
            var resource = await _context.ModuleResource.FindAsync(id);
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

            resource!.UpdatedByUserId = user.Id;
            resource.UpdatedAt = DateTime.UtcNow;
            resource.Description = ok.Value.Description;
            resource.Name = ok.Value.Name;
            resource.URL = ok.Value.URL;
            resource.ResourceType = Tools.ParseResourceType(ok.Value.ResourceType);
            await _context.SaveChangesAsync();

            return Ok(resource.ToResourceDto());
        }

        [HttpDelete("api/modules/resources/{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<IActionResult> DeleteModuleResource(int id)
        {
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();

            var resource = await _context.ModuleResource.FindAsync(id);

            var decision = ResourceWorkflow.ValidateForDelete(caller, resource is not null, id);
            if (decision is not WorkflowResult<Unit>.Ok)
                return this.ToActionResult(decision);

            _context.ModuleResource.Remove(resource!);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}