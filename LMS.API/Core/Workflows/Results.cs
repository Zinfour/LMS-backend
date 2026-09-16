namespace LMS.API.Core.Workflows;

public abstract record WorkflowResult<T>
{
    public sealed record Ok(T Value) : WorkflowResult<T>;
    public sealed record Unauthorized(string Reason) : WorkflowResult<T>;
    public sealed record Forbidden(string Reason) : WorkflowResult<T>;
    public sealed record BadRequest(string Reason) : WorkflowResult<T>;
    public sealed record NotFound(string Reason) : WorkflowResult<T>;
    public sealed record Conflict(string Reason) : WorkflowResult<T>;
    public sealed record ValidationFailed(IReadOnlyList<string> Errors) : WorkflowResult<T>;
}