using System.Security.Claims;
using LMS.API.Core.Types;
using LMS.API.Core.Workflows;
using LMS.API.Data;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Shell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(string? role, int? course)
        {
            if (role is not null && role is not (Role.Teacher or Role.Student))
                return BadRequest($"Invalid role: {role}.");

            if (course is not null &&
                !await _context.Course.AnyAsync(c => c.Id == course))
                return BadRequest("Invalid CourseId.");

            var users = await _context.Users
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
                    CourseId = u.CourseId
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isTeacher = User.IsInRole(Role.Teacher);

            if (!isTeacher && callerId != id) return Forbid();

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
                    CourseId = u.CourseId
                })
                .FirstOrDefaultAsync();

            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> CreateUser(CreateUserDto dto)
        {
            var roleValidation = UserWorkflow.ValidateRole(dto.Role);
            if (roleValidation is not WorkflowResult<string>.Ok ok)
                return this.ToActionResult(roleValidation);

            if (!await _context.Course.AnyAsync(c => c.Id == dto.CourseId))
                return BadRequest("Invalid CourseId.");

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
                CourseId = dto.CourseId
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors.Select(e => e.Description));

            var roleResult = await _userManager.AddToRoleAsync(user, ok.Value);
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors.Select(e => e.Description));

            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = dto.Role,
                ImageUrl = user.ImageUrl,
                CourseId = user.CourseId
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> UpdateUser(string id, UpdateUserDto dto)
        {
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(id);
            var courseExists = await _context.Course.AnyAsync(c => c.Id == dto.CourseId);
            var currentRoles = user is not null
                ? (IReadOnlyList<string>)await _userManager.GetRolesAsync(user)
                : Array.Empty<string>();

            var plan = UserWorkflow.PlanUpdate(new UserUpdateInput(
                Caller: caller,
                TargetUserId: id,
                UserExists: user is not null,
                CourseExists: courseExists,
                NewRole: dto.Role,
                NewPassword: dto.Password,
                CurrentRoles: currentRoles));

            if (plan is not WorkflowResult<UserUpdatePlan>.Ok ok)
                return this.ToActionResult(plan);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            user!.UpdatedAt = DateTime.UtcNow;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.ImageUrl = dto.ImageUrl;
            user.CourseId = dto.CourseId;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            if (ok.Value.ChangePassword)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.Password!);
                if (!resetResult.Succeeded)
                    return BadRequest(resetResult.Errors.Select(e => e.Description));
            }

            if (ok.Value.ChangeRole)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return BadRequest(removeResult.Errors.Select(e => e.Description));

                var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addResult.Succeeded)
                    return BadRequest(addResult.Errors.Select(e => e.Description));
            }

            await transaction.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(id);

            var decision = UserWorkflow.ValidateDelete(new UserDeleteInput(
                Caller: caller,
                TargetUserId: id,
                UserExists: user is not null));

            if (decision is not WorkflowResult<Unit>.Ok)
                return this.ToActionResult(decision);

            var result = await _userManager.DeleteAsync(user!);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return NoContent();
        }
    }
}