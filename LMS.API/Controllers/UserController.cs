using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(LmsContext lmsContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly LmsContext _context = lmsContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(string? role, int? course)
    {
        if (role != null && role != Role.Teacher && role != Role.Student)
        {
            return BadRequest($"Invalid role: {role}.");
        }

        if (course != null)
        {
            var courseExists = await _context.Course.AnyAsync(c => c.Id == course);
            if (!courseExists)
            {
                return BadRequest("Invalid CourseId.");
            }
        }

        return await _context.Users
            .Where(u => course == null || u.CourseId == course)
            .Where(u => role == null || u.Roles.Any(r => r.Name == role))
            .Select(u => new UserDto
            {
                Id = u.Id,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Email = u.Email!,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Roles.Select(r => r.Name).FirstOrDefault()!,
                ImageUrl = u.ImageUrl,
                CourseId = u.CourseId,
            }).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Email = u.Email!,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Roles.Select(r => r.Name).FirstOrDefault()!,
                ImageUrl = u.ImageUrl,
                CourseId = u.CourseId,
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDto dto)
    {
        if (dto.Role != Role.Teacher && dto.Role != Role.Student)
        {
            return BadRequest($"Invalid role: {dto.Role}.");
        }

        var course = await _context.Course.FindAsync(dto.CourseId);
        if (course == null)
        {
            return BadRequest("Invalid CourseId.");
        }

        var now = DateTime.UtcNow;

        var user = new ApplicationUser
        {
            CreatedAt = now,
            UpdatedAt = now,
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ImageUrl = dto.ImageUrl,
            CourseId = dto.CourseId,
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(createResult.Errors.Select(e => e.Description));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user); // don't leave a role-less account behind
            return BadRequest(roleResult.Errors.Select(e => e.Description));
        }

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            new UserDto
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = dto.Role,
                ImageUrl = user.ImageUrl,
                CourseId = user.CourseId,
            });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(string id, UpdateUserDto dto)
    {
        if (dto.Role != Role.Teacher && dto.Role != Role.Student)
        {
            return BadRequest($"Invalid role: {dto.Role}.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var course = await _context.Course.FindAsync(dto.CourseId);
        if (course == null)
        {
            return BadRequest("Invalid CourseId.");
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.ImageUrl = dto.ImageUrl;
        user.CourseId = dto.CourseId;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(updateResult.Errors.Select(e => e.Description));
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

            if (!resetResult.Succeeded)
            {
                return BadRequest(resetResult.Errors.Select(e => e.Description));
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(dto.Role))
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                return BadRequest(removeRolesResult.Errors.Select(e => e.Description));
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!addRoleResult.Succeeded)
            {
                return BadRequest(addRoleResult.Errors.Select(e => e.Description));
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return NoContent();
    }
}
