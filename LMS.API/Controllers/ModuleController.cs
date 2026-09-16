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
    [Route("/api/courses/{courseId}/modules")]
    public class ModuleController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        // ---------- GET /api/courses/{courseId}/modules ----------
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("User not found.");
            var roles = await _userManager.GetRolesAsync(user);
            var caller = new CallerContext(user.Id,
                roles.Contains(Role.Teacher), roles.Contains(Role.Student), user.CourseId);

            var courseExists = await _context.Course.AnyAsync(c => c.Id == courseId);

            // Core: pure resolution of which course to read.
            var resolved = ModuleWorkflow.ResolveCourse(caller, courseId, courseExists);
            if (resolved is not WorkflowResult<int>.Ok ok)
                return this.ToActionResult(resolved);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Shell: load the raw numbers, then let the pure workflow decide status.
            var raw = await _context.Module
                .Where(m => m.CourseId == ok.Value)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Description,
                    m.StartDate,
                    m.EndDate,
                    m.ImageURL,
                    m.CourseId,
                    ActivitiesNumber = m.Activities.Count,
                    ResourcesNumber = m.Resources.Count,
                    NumberOfCompletedActivities =
                        m.Activities.Count(a => a.CompletedUsers.Any(u => u.Id == user.Id)),
                })
                .ToListAsync();

            var result = raw.Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                ImageURL = m.ImageURL,
                CourseId = m.CourseId,
                Order = _context.Module.Count(md => md.CourseId == m.CourseId && md.StartDate < m.StartDate),
                ActivitiesNumber = m.ActivitiesNumber,
                ResourcesNumber = m.ResourcesNumber,
                NumberOfCompletedActivities = m.NumberOfCompletedActivities,
                CurrentStatus = ModuleStatusWorkflow.Calculate(
                    m.StartDate, m.EndDate, m.ActivitiesNumber, m.NumberOfCompletedActivities, today)
            }).ToList();

            return Ok(result);
        }

        // ---------- GET /api/courses/{courseId}/modules/{moduleId} ----------
        [HttpGet("{moduleId}")]
        [Authorize]
        public async Task<ActionResult<ModuleFullDto>> GetModule(int courseId, int moduleId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return BadRequest("User not found.");
            var roles = await _userManager.GetRolesAsync(user);
            var caller = new CallerContext(user.Id,
                roles.Contains(Role.Teacher), roles.Contains(Role.Student), user.CourseId);

            var courseExists = await _context.Course.AnyAsync(c => c.Id == courseId);

            var resolved = ModuleWorkflow.ResolveCourse(caller, courseId, courseExists);
            if (resolved is not WorkflowResult<int>.Ok ok)
                return this.ToActionResult(resolved);

            // Shell: load aggregate with everything we need for a pure projection.
            var module = await _context.Module
                .Include(m => m.Activities).ThenInclude(a => a.Assignment).ThenInclude(a => a!.Submissions)
                .Include(m => m.Activities).ThenInclude(a => a.Resources)
                .Include(m => m.Resources)
                .Include(m => m.Course).ThenInclude(c => c.Modules)
                .FirstOrDefaultAsync(m => m.CourseId == ok.Value && m.Id == moduleId);

            // Core: pure access decision on the specific module.
            var access = ModuleWorkflow.ValidateModuleAccess(new ModuleReadContext(
                Caller: caller,
                ResolvedCourseId: ok.Value,
                CourseExists: true,
                ModuleExists: module is not null));

            if (access is not WorkflowResult<Unit>.Ok)
                return this.ToActionResult(access);

            // Shell: pure projection helpers.
            var dto = new ModuleFullDto
            {
                Id = module!.Id,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt,
                Name = module.Name,
                Description = module.Description,
                StartDate = module.StartDate,
                EndDate = module.EndDate,
                ImageURL = module.ImageURL,
                CourseId = module.CourseId,
                Order = await _context.Module.CountAsync(m => m.CourseId == module.CourseId && m.StartDate < module.StartDate),
                TotalNumberOfModules = module.Course.Modules.Count,
                Activities = module.Activities.Select(a => a.ToActivityDto()).ToList(),
                Resources = module.Resources.Select(r => r.ToResourceDto()).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> CreateModule(int courseId, CreateModuleDto createModuleDto)
        {
            var course = await _context.Course.Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            var validationResult = ModuleWorkflow.ValidateForCreate(
                new ModuleWriteContext(
                Caller: CurrentUser.From(User)!,
                ParentCourseId: courseId,
                CourseStartDate: course?.StartDate ?? default,
                CourseEndDate: course?.EndDate ?? default,
                CourseExists: course is not null,
                ModuleExists: true,
                ExistingModules: course?.Modules.Select(m => new ModuleWrite
                (
                    Id: m.Id,
                    Name: m.Name,
                    Description: m.Description,
                    StartDate: m.StartDate,
                    EndDate: m.EndDate,
                    ImageURL: m.ImageURL
                )).ToList(),
                Write: new ModuleWrite
                (
                    Id: 0,
                    Name: createModuleDto.Name,
                    Description: createModuleDto.Description,
                    StartDate: createModuleDto.StartDate,
                    EndDate: createModuleDto.EndDate,
                    ImageURL: createModuleDto.ImageURL
                )));

            if (validationResult is not WorkflowResult<ModuleWrite>.Ok)
            {
                return this.ToActionResult(validationResult);
            }

            var module = new Module
            {
                Name = createModuleDto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Description = createModuleDto.Description,
                StartDate = createModuleDto.StartDate,
                EndDate = createModuleDto.EndDate,
                ImageURL = createModuleDto.ImageURL,
                CourseId = courseId,
                Course = course!
            };

            _context.Module.Add(module);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(CreateModule),
                new { id = module.Id },
                module.ToModuleFullDto(course!.Modules.Count)
                );
        }

        [HttpPut("{moduleId}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> updateModule(int courseId, int moduleId, UpdateModuleDto updateModuleDto)
        {
            var course = await _context.Course.Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            var module = course?.Modules.FirstOrDefault(m => m.Id == moduleId);

            var validationResult = ModuleWorkflow.ValidateForUpdate(
                new ModuleWriteContext(
                Caller: CurrentUser.From(User)!,
                ParentCourseId: courseId,
                CourseStartDate: course?.StartDate ?? default,
                CourseEndDate: course?.EndDate ?? default,
                CourseExists: course is not null,
                ModuleExists: module is not null,
                ExistingModules: course?.Modules.Select(m => new ModuleWrite
                (
                    Id: m.Id,
                    Name: m.Name,
                    Description: m.Description,
                    StartDate: m.StartDate,
                    EndDate: m.EndDate,
                    ImageURL: m.ImageURL
                )).ToList(),
                Write: new ModuleWrite
                (
                    Id: moduleId,
                    Name: updateModuleDto.Name,
                    Description: updateModuleDto.Description,
                    StartDate: updateModuleDto.StartDate,
                    EndDate: updateModuleDto.EndDate,
                    ImageURL: updateModuleDto.ImageURL
                )));

            if (validationResult is not WorkflowResult<ModuleWrite>.Ok)
            {
                return this.ToActionResult(validationResult);
            }

            module!.Id = updateModuleDto.Id;
            module.UpdatedAt = DateTime.UtcNow;
            module.Name = updateModuleDto.Name;
            module.Description = updateModuleDto.Description;
            module.StartDate = updateModuleDto.StartDate;
            module.EndDate = updateModuleDto.EndDate;
            module.ImageURL = updateModuleDto.ImageURL;
            module.CourseId = courseId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ---------- DELETE /api/courses/{courseId}/modules/{moduleId} ----------
        [HttpDelete("{moduleId}")]
        [Authorize(Roles = Role.Teacher)]
        public async Task<ActionResult> DeleteModule(int courseId, int moduleId)
        {
            if (!_context.Course.Any(c => c.Id == courseId))
            {
                return NotFound($"Couldn't find course with id: {courseId}");
            }

            var moduleToDelete = await _context.Module.FirstOrDefaultAsync(m => m.Id == moduleId && m.CourseId == courseId);

            if (moduleToDelete == null)
            {
                return NotFound($"Couldn't find module with id: {moduleId}");
            }

            _context.Module.Remove(moduleToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}