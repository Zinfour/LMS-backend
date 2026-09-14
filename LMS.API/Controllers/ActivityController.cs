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

        // --- Read: GET /api/activities ---
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

        // --- Read: GET /api/activities/{activityId} ---
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

        [HttpDelete]
        [Authorize(Roles = Role.Teacher)]
        [Route("activityId")]
        public async Task<ActionResult> DeleteActivity(int activityId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var isTeacher = User.IsInRole(Role.Teacher);
            var isStudent = User.IsInRole(Role.Student);

            if (!isTeacher)
            {
                return Unauthorized();
            }
            else if (!isStudent)
            {
                return BadRequest("Invalid role.");
            }
            var activityToDelete = await _context.Activity.FirstOrDefaultAsync(a => a.Id == activityId);

            if (activityToDelete == null)
            {
                return NotFound($"Couldn't find activity with id: {activityId}");
            }

            _context.Activity.Remove(activityToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- Command: POST /api/activities/{activityId}/complete/{userId} ---
        [HttpPost("{activityId}/complete/{userId}")]
        [Authorize]
        public async Task<ActionResult> CompleteActivity(int activityId, string userId)
        {
            var caller = CurrentUser.From(User);
            if (caller is null) return Unauthorized();

            // Shell: gather facts.
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

            // Shell: if the pure core says OK, apply the mutation.
            if (decision is WorkflowResult<Unit>.Ok && !alreadyCompleted && activity is not null && user is not null)
            {
                activity.CompletedUsers.Add(user);
                await _context.SaveChangesAsync();
            }

            return this.ToActionResult(decision);
        }

        // --- Placeholders untouched ---
        [HttpPost][Authorize(Roles = Role.Teacher)] public ActionResult CreateActivity() => BadRequest();
        [HttpPut][Authorize(Roles = Role.Teacher)] public ActionResult UpdateActivity() => BadRequest();
        [HttpDelete][Authorize(Roles = Role.Teacher)] public ActionResult DeleteActivity() => BadRequest();
    }
}