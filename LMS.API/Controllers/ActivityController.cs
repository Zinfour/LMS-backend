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
    [Route("/api/activities")]
    public class ActivityController(
        LmsContext lmsContext,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly LmsContext _context = lmsContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities(
            [FromQuery] int? courseId, [FromQuery] int? moduleId)
        {
            var caller = CurrentUser.From(User);
            if (AccessPolicy.DenyIfNotAuthenticated<IEnumerable<ActivityDto>>(caller) is { } unauth)
                return unauth switch
                {
                    WorkflowResult<IEnumerable<ActivityDto>>.Unauthorized u => Unauthorized(u.Reason),
                    _ => StatusCode(500)
                };

            if (AccessPolicy.DenyIfInvalidRole<IEnumerable<ActivityDto>>(caller!) is { } roleFail)
                return BadRequest(((WorkflowResult<IEnumerable<ActivityDto>>.BadRequest)roleFail).Reason);

            // Shell: build the query and execute it.
            var query = _context.Activity
                .Where(a =>
                    (courseId == null || a.Module.CourseId == courseId) &&
                    (moduleId == null || a.ModuleId == moduleId) &&
                    (caller!.IsTeacher || a.Module.Course.Users.Any(u => u.Id == caller.UserId)))
                .Include(a => a.Assignment).ThenInclude(ass => ass == null ? null : ass.Submissions)
                .Include(a => a.Resources);

            var activities = await query
                .Select(a => a.ToActivityDto())
                .ToListAsync();

            return Ok(activities);
        }

        [HttpGet("{activityId}")]
        [Authorize]
        public async Task<ActionResult<ActivityDto>> GetActivity(int activityId)
        {
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();
            if (!caller.IsTeacher && !caller.IsStudent) return BadRequest("Invalid role.");

            var activity = await _context.Activity
                .Where(a => a.Id == activityId &&
                            (caller.IsTeacher ||
                             a.Module.Course.Users.Any(u => u.Id == caller.UserId)))
                .Include(a => a.Assignment).ThenInclude(ass => ass == null ? null : ass.Submissions)
                .Include(a => a.Resources)
                .FirstOrDefaultAsync();

            var activityDto = activity is null ? null : activity.ToActivityDto();

            return activityDto is null
                ? NotFound($"Couldn't find activity with id: {activityId}.")
                : Ok(activityDto);
        }

        [HttpPost("{activityId}/complete/{userId}")]
        [Authorize]
        public async Task<ActionResult> CompleteActivity(int activityId, string userId)
        {
            // Shell: IO
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();

            // Shell: IO
            var user = await _userManager.FindByIdAsync(userId);
            var activity = await _context.Activity
                .Include(a => a.CompletedUsers)
                .FirstOrDefaultAsync(a => a.Id == activityId);
            var alreadyCompleted = activity?.CompletedUsers.Any(u => u.Id == userId) ?? false;

            // Core: pure decision.
            var decision = CompleteActivityWorkflow.Execute(new CompleteActivityInput(
                Caller: caller,
                TargetUserId: userId,
                ActivityId: activityId,
                UserExists: user is not null,
                ActivityExists: activity is not null,
                AlreadyCompleted: alreadyCompleted));

            // Shell: IO
            if (decision is WorkflowResult<Unit>.Ok && !alreadyCompleted && activity is not null && user is not null)
            {
                activity.CompletedUsers.Add(user);
                await _context.SaveChangesAsync();
            }

            return this.ToActionResult(decision);
        }

        
        [HttpPost]
        [Authorize(Roles = Role.Teacher)] 
        public async Task<ActionResult> CreateActivity([FromBody] CreateActivityDto createActivityDto)
        {
            var module = await _context.Module.Where(m => m.Id == createActivityDto.ModuleId).Include(m => m.Activities).FirstOrDefaultAsync();

            var validationResult = ActivityWorkflow.ValidateForCreate(new ActivityContext(
                Caller: CurrentUser.From(User)!,
                ParentModuleExists: module is not null,
                ParentModuleId: createActivityDto.ModuleId,
                ModuleStartDate: module == null ? DateOnly.MinValue : module.StartDate,
                ModuleEndDate: module == null ? DateOnly.MinValue : module.EndDate,
                ExistingActivities: (IReadOnlyList<ActivityWrite>?)module?.Activities.Select(a => new ActivityWrite(
                    Id: a.Id,
                    Name: a.Name,
                    Description: a.Description,
                    StartTime: a.StartTime,
                    EndTime: a.EndTime,
                    ActivityExists: true
                )).ToList(),
                Write: new ActivityWrite(
                    Id: 0,
                    Name: createActivityDto.Name,
                    Description: createActivityDto.Description,
                    StartTime: createActivityDto.StartTime,
                    EndTime: createActivityDto.EndTime,
                    ActivityExists: true
                )
            ));

            if(validationResult is not WorkflowResult<ActivityWrite>.Ok ok)
            {
                return this.ToActionResult(validationResult);
            }

            var newActivity = new Activity
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Type = createActivityDto.Type,
                Name = createActivityDto.Name,
                Description = createActivityDto.Description,
                StartTime = createActivityDto.StartTime,
                EndTime = createActivityDto.EndTime,
                ModuleId = createActivityDto.ModuleId
            };
            _context.Activity.Add(newActivity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { activityId = newActivity.Id }, newActivity.ToActivityDto());
        }

        
        [HttpPut("{activityId}")]
        [Authorize(Roles = Role.Teacher)] 
        public async Task<ActionResult> UpdateActivityAsync(int activityId, [FromBody] UpdateActivityDto updateActivityDto)
        {
            var module = await _context.Module.Where(m => m.Id == updateActivityDto.ModuleId).Include(m => m.Activities).FirstOrDefaultAsync();

            var existingActivity = await _context.Activity.FirstOrDefaultAsync(a => a.Id == activityId);

            var validationResult = ActivityWorkflow.ValidateForUpdate(new ActivityContext(
                Caller: CurrentUser.From(User)!,
                ParentModuleExists: module is not null,
                ParentModuleId: updateActivityDto.ModuleId,
                ModuleStartDate: module == null ? DateOnly.MinValue : module.StartDate,
                ModuleEndDate: module == null ? DateOnly.MinValue : module.EndDate,
                ExistingActivities: (IReadOnlyList<ActivityWrite>?)module?.Activities.Select(a => new ActivityWrite(
                    Id: a.Id,
                    Name: a.Name,
                    Description: a.Description,
                    StartTime: a.StartTime,
                    EndTime: a.EndTime,
                    ActivityExists: true
                )).ToList(),
                Write: new ActivityWrite(
                    Id: updateActivityDto.Id,
                    Name: updateActivityDto.Name,
                    Description: updateActivityDto.Description,
                    StartTime: updateActivityDto.StartTime,
                    EndTime: updateActivityDto.EndTime,
                    ActivityExists: existingActivity != null
                )
            ));

            if (validationResult is not WorkflowResult<ActivityWrite>.Ok ok)
            {
                return this.ToActionResult(validationResult);
            }

            if(existingActivity != null) 
            {
                existingActivity.UpdatedAt = DateTime.UtcNow;
                existingActivity.Type = updateActivityDto.Type;
                existingActivity.Name = updateActivityDto.Name;
                existingActivity.Description = updateActivityDto.Description;
                existingActivity.ImageURL = updateActivityDto.ImageURL;
                existingActivity.StartTime = updateActivityDto.StartTime;
                existingActivity.EndTime = updateActivityDto.EndTime;
                existingActivity.ModuleId = updateActivityDto.ModuleId;

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = Role.Teacher)]
        [Route("activityId")]
        public async Task<ActionResult> DeleteActivity(int activityId)
        {
            var activityToDelete = await _context.Activity.FirstOrDefaultAsync(a => a.Id == activityId);

            if (activityToDelete == null)
            {
                return NotFound($"Couldn't find activity with id: {activityId}");
            }

            _context.Activity.Remove(activityToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}