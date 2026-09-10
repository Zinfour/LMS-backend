using LMS.API.Data;
using LMS.API.DTOs.Resource;
using LMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
public class ModuleResourcesController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet("api/modules/{id}/resources")]
    [Authorize(Roles = Role.Teacher + "," + Role.Student)]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetModuleResources(int id)
    {
        var resources = await _context.ModuleResource.Where(r => r.ModuleId == id).ToListAsync();
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
        });
        return Ok(resourceDtos);
    }

    [HttpGet("api/modules/resources/{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<ResourceDto>> GetModuleResource(int id)
    {
        var resource = await _context.ModuleResource.FindAsync(id);
        if (resource == null)
        {
            return NotFound($"Module resource with ID {id} not found.");
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
        };

        return Ok(resourceDto);
    }

    [HttpPost("api/modules/{id}/resources")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<ResourceDto>> CreateModuleResource(int id, [FromBody] CreateResourceDto createResourceDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("Logged-in User not found.");
        }

        var module = await _context.Module.FindAsync(id);
        if (module == null)
        {
            return NotFound($"Module with ID {id} not found.");
        }

        var resource = new ModuleResource
        {
            ModuleId = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            UpdatedByUserId = user.Id,
            Description = createResourceDto.Description,
            Name = createResourceDto.Name,
            URL = createResourceDto.URL,
            ResourceType = Tools.ParseResourceType(createResourceDto.ResourceType.ToString())
        };

        _context.ModuleResource.Add(resource);
        await _context.SaveChangesAsync();

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
        };

        return CreatedAtAction(nameof(GetModuleResource), new { id = resource.Id }, resourceDto);
    }

    [HttpPut("api/modules/resources/{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<ActionResult<ResourceDto>> UpdateModuleResource(int id, [FromBody] UpdateResourceDto resourceDto)
    {
        var resource = await _context.ModuleResource.FindAsync(id);
        if (resource == null)
        {
            return NotFound($"Module resource with ID {id} not found.");
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
        };

        return Ok(result);
    }

    [HttpDelete("api/modules/resources/{id}")]
    [Authorize(Roles = Role.Teacher)]
    public async Task<IActionResult> DeleteModuleResource(int id)
    {
        var resource = await _context.ModuleResource.FindAsync(id);
        if (resource == null)
        {
            return NotFound($"Module resource with ID {id} not found.");
        }

        _context.ModuleResource.Remove(resource);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
