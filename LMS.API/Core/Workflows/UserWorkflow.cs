using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class UserWorkflow
{
    public static WorkflowResult<string> ValidateRole(string role)
        => role is "Teacher" or "Student"
            ? new WorkflowResult<string>.Ok(role)
            : new WorkflowResult<string>.BadRequest($"Invalid role: {role}.");

    public static WorkflowResult<UserUpdatePlan> PlanUpdate(UserUpdateInput input)
    {
        if (!input.Caller.IsTeacher)
            return new WorkflowResult<UserUpdatePlan>.Forbidden("Only teachers may update users.");

        if (!input.UserExists)
            return new WorkflowResult<UserUpdatePlan>.NotFound("User not found.");

        if (!input.CourseExists)
            return new WorkflowResult<UserUpdatePlan>.BadRequest("Invalid CourseId.");

        if (input.NewRole is not ("Teacher" or "Student"))
            return new WorkflowResult<UserUpdatePlan>.BadRequest($"Invalid role: {input.NewRole}.");

        var roleChanged = !input.CurrentRoles.Contains(input.NewRole);

        return new WorkflowResult<UserUpdatePlan>.Ok(new UserUpdatePlan(
            ChangePassword: !string.IsNullOrEmpty(input.NewPassword),
            ChangeRole: roleChanged));
    }

    public static WorkflowResult<Unit> ValidateDelete(UserDeleteInput input)
    {
        if (!input.Caller.IsTeacher)
            return new WorkflowResult<Unit>.Forbidden("Only teachers may delete users.");

        if (!input.UserExists)
            return new WorkflowResult<Unit>.NotFound("User not found.");

        return new WorkflowResult<Unit>.Ok(Unit.Value);
    }
}

public record UserUpdatePlan(bool ChangePassword, bool ChangeRole);