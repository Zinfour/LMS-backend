using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class AccessPolicy
{
    public static WorkflowResult<T>? DenyIfNotAuthenticated<T>(CallerContext? caller)
        => caller is null
            ? new WorkflowResult<T>.Unauthorized("Not authenticated.")
            : null;

    public static WorkflowResult<T>? DenyIfInvalidRole<T>(CallerContext caller)
        => !caller.IsTeacher && !caller.IsStudent
            ? new WorkflowResult<T>.BadRequest("Invalid role.")
            : null;

    public static WorkflowResult<T>? DenyIfNotSelfOrTeacher<T>(CallerContext caller, string targetUserId)
        => !caller.IsTeacher && caller.UserId != targetUserId
            ? new WorkflowResult<T>.Forbidden("Not allowed to act on behalf of another user.")
            : null;
}