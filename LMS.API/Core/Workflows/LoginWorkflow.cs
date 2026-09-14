using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class LoginWorkflow
{
    // Pure: given everything the shell already loaded, decide if login succeeds
    // and produce the payload the token should carry.
    public static WorkflowResult<LoginPayload> Execute(LoginInput input)
    {
        if (input.FoundUserId is null)
            return new WorkflowResult<LoginPayload>.Unauthorized("User does not exist.");

        if (!input.PasswordMatches)
            return new WorkflowResult<LoginPayload>.Unauthorized("Invalid password.");

        if (input.Profile is null)
            return new WorkflowResult<LoginPayload>.Unauthorized("User profile missing.");

        var role = input.Roles.FirstOrDefault() ?? "Student";

        return new WorkflowResult<LoginPayload>.Ok(
            new LoginPayload(input.Profile, role));
    }
}

public record LoginPayload(UserProfile Profile, string Role);