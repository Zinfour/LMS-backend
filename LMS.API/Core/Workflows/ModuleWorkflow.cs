using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class ModuleWorkflow
{
    public static WorkflowResult<int> ResolveCourse(
        CallerContext? caller,
        int requestedCourseId,
        bool courseExists)
    {
        if (caller is null)
            return new WorkflowResult<int>.Unauthorized("Not authenticated.");

        if (!caller.IsTeacher && !caller.IsStudent)
            return new WorkflowResult<int>.BadRequest("Invalid role.");

        var resolved = CourseAccess.ResolveCourseId(caller, requestedCourseId);
        if (resolved is null)
            return new WorkflowResult<int>.BadRequest("Invalid role.");

        if (!courseExists)
            return new WorkflowResult<int>.BadRequest("Invalid CourseId.");

        return new WorkflowResult<int>.Ok(resolved.Value);
    }

    public static WorkflowResult<Unit> ValidateModuleAccess(ModuleReadContext ctx)
    {
        if (ctx.Caller is null)
            return new WorkflowResult<Unit>.Unauthorized("Not authenticated.");
        if (!ctx.Caller.IsTeacher && !ctx.Caller.IsStudent)
            return new WorkflowResult<Unit>.BadRequest("Invalid role.");
        if (!ctx.CourseExists)
            return new WorkflowResult<Unit>.BadRequest("Invalid CourseId.");
        if (!ctx.ModuleExists)
            return new WorkflowResult<Unit>.BadRequest("Invalid ModuleId");
        return new WorkflowResult<Unit>.Ok(Unit.Value);
    }
}