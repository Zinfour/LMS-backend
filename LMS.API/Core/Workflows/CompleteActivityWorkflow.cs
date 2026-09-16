using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class CompleteActivityWorkflow
{
    public static WorkflowResult<Unit> Validate(CompleteActivityInput input)
    {
        if (AccessPolicy.DenyIfInvalidRole<Unit>(input.Caller) is { } roleFail) return roleFail;
        if (AccessPolicy.DenyIfNotSelfOrTeacher<Unit>(input.Caller, input.TargetUserId) is { } authFail) return authFail;

        if (!input.UserExists)
            return new WorkflowResult<Unit>.NotFound($"Couldn't find user with id: {input.TargetUserId}.");
        if (!input.ActivityExists)
            return new WorkflowResult<Unit>.NotFound($"Couldn't find activity with id: {input.ActivityId}.");
        if (input.AlreadyCompleted)
            return new WorkflowResult<Unit>.Ok(Unit.Value);

        // The "add user to CompletedUsers" effect will be performed by the shell
        // because it mutates the aggregate. We just say it's allowed.
        return new WorkflowResult<Unit>.Ok(Unit.Value);
    }
}

public readonly record struct Unit { public static Unit Value => default; }