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

    public static WorkflowResult<ModuleWrite> ValidateForCreate(ModuleWriteContext ctx)
    {
        return ValidateForCreateAndUpdate(ctx);
    }

    public static WorkflowResult<ModuleWrite> ValidateForUpdate(ModuleWriteContext ctx)
    {
        if (!ctx.ModuleExists)
            return new WorkflowResult<ModuleWrite>.NotFound($"Module with ID {ctx.Write.Id} not found.");

        return ValidateForCreateAndUpdate(ctx);
    }

    private static WorkflowResult<ModuleWrite> ValidateForCreateAndUpdate(ModuleWriteContext ctx)
    {
        if (!ctx.CourseExists)
            return new WorkflowResult<ModuleWrite>.NotFound(
                $"Parent course with ID {ctx.ParentCourseId} not found.");

        if(ctx.Write.StartDate > ctx.Write.EndDate)
            return new WorkflowResult<ModuleWrite>.BadRequest(
                "Module start date must be before or equal to the end date.");

        if (ctx.Write.StartDate < ctx.CourseStartDate || ctx.Write.EndDate > ctx.CourseEndDate)
            return new WorkflowResult<ModuleWrite>.BadRequest(
                $"Module dates must be within the course dates ({ctx.CourseStartDate} to {ctx.CourseEndDate}).");

        var overlappingModule = ctx.ExistingModules?.Where(m =>
            m.Id != ctx.Write.Id &&
            ((ctx.Write.StartDate >= m.StartDate && ctx.Write.StartDate <= m.EndDate) ||
             (ctx.Write.EndDate >= m.StartDate && ctx.Write.EndDate <= m.EndDate) ||
             (ctx.Write.StartDate <= m.StartDate && ctx.Write.EndDate >= m.EndDate))).FirstOrDefault();

        if (overlappingModule != null)
            return new WorkflowResult<ModuleWrite>.BadRequest(
                $"Module dates conflict with existing module '{overlappingModule.Name}' ({overlappingModule.Id}).");

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(ctx.Write.Name)) errors.Add("Name is required.");
        if (string.IsNullOrWhiteSpace(ctx.Write.Description)) errors.Add("Description is required.");
        if (ctx.Write.StartDate == DateOnly.MinValue) errors.Add("StartTime is required.");
        if (ctx.Write.EndDate == DateOnly.MinValue) errors.Add("EndTime is required.");

        return errors.Count == 0
            ? new WorkflowResult<ModuleWrite>.Ok(ctx.Write)
            : new WorkflowResult<ModuleWrite>.ValidationFailed(errors);
    }
}