using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class ResourceWorkflow
{
    public static WorkflowResult<ResourceWrite> Validate(ResourceWrite write)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(write.Name)) errors.Add("Name is required.");
        if (string.IsNullOrWhiteSpace(write.ResourceType)) errors.Add("ResourceType is required.");
        // Tools.ParseResourceType has a default, so no failure on unknown type, but you could reject:
        // if (!KnownTypes.Contains(write.ResourceType)) errors.Add("Unknown ResourceType.");

        return errors.Count == 0
            ? new WorkflowResult<ResourceWrite>.Ok(write)
            : new WorkflowResult<ResourceWrite>.ValidationFailed(errors);
    }

    public static WorkflowResult<ResourceWrite> ValidateForCreate(ResourceWriteContext ctx)
    {
        if (!ctx.Caller.IsTeacher)
            return new WorkflowResult<ResourceWrite>.Forbidden("Only teachers may modify resources.");

        if (!ctx.ParentExists)
            return new WorkflowResult<ResourceWrite>.NotFound(
                $"Parent entity with ID {ctx.ParentId} not found.");

        return Validate(ctx.Write);
    }

    public static WorkflowResult<ResourceWrite> ValidateForUpdate(ResourceWriteContext ctx)
    {
        if (!ctx.Caller.IsTeacher)
            return new WorkflowResult<ResourceWrite>.Forbidden("Only teachers may modify resources.");

        if (!ctx.ParentExists)
            return new WorkflowResult<ResourceWrite>.NotFound(
                $"Resource with ID {ctx.ParentId} not found.");

        return Validate(ctx.Write);
    }

    public static WorkflowResult<Unit> ValidateForDelete(CallerContext caller, bool resourceExists, int resourceId)
    {
        if (!caller.IsTeacher)
            return new WorkflowResult<Unit>.Forbidden("Only teachers may delete resources.");

        if (!resourceExists)
            return new WorkflowResult<Unit>.NotFound($"Resource with ID {resourceId} not found.");

        return new WorkflowResult<Unit>.Ok(Unit.Value);
    }
}