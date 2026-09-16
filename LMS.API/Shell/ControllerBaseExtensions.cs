using LMS.API.Core.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Shell;

public static class ControllerBaseExtensions
{
    public static ActionResult ToActionResult<T>(this ControllerBase c, WorkflowResult<T> result)
        => result switch
        {
            WorkflowResult<T>.Ok ok => c.Ok(ok.Value),
            WorkflowResult<T>.Unauthorized u => c.Unauthorized(u.Reason),
            WorkflowResult<T>.Forbidden f => c.Forbid(),
            WorkflowResult<T>.BadRequest b => c.BadRequest(b.Reason),
            WorkflowResult<T>.NotFound n => c.NotFound(n.Reason),
            WorkflowResult<T>.Conflict c2 => c.Conflict(c2.Reason),
            WorkflowResult<T>.ValidationFailed v => c.BadRequest(v.Errors),
            _ => c.StatusCode(500)
        };

    public static ActionResult ToActionResult(this ControllerBase c, WorkflowResult<Unit> result)
        => result switch
        {
            WorkflowResult<Unit>.Ok => c.NoContent(),
            WorkflowResult<Unit>.Unauthorized u => c.Unauthorized(u.Reason),
            WorkflowResult<Unit>.Forbidden => c.Forbid(),
            WorkflowResult<Unit>.BadRequest b => c.BadRequest(b.Reason),
            WorkflowResult<Unit>.NotFound n => c.NotFound(n.Reason),
            WorkflowResult<Unit>.Conflict c2 => c.Conflict(c2.Reason),
            WorkflowResult<Unit>.ValidationFailed v => c.BadRequest(v.Errors),
            _ => c.StatusCode(500)
        };

    //public static ActionResult<T> ToActionResult<T>(this ControllerBase c, WorkflowResult<T> result)
    //    => result switch
    //    {
    //        WorkflowResult<T>.Ok ok =>
    //            ok.Value is Unit
    //                ? c.NoContent()
    //                : c.Ok(ok.Value),

    //        WorkflowResult<T>.Unauthorized u => c.Unauthorized(u.Reason),
    //        WorkflowResult<T>.Forbidden => c.Forbid(),
    //        WorkflowResult<T>.BadRequest b => c.BadRequest(b.Reason),
    //        WorkflowResult<T>.NotFound n => c.NotFound(n.Reason),
    //        WorkflowResult<T>.Conflict c2 => c.Conflict(c2.Reason),
    //        WorkflowResult<T>.ValidationFailed v => c.BadRequest(v.Errors),
    //        _ => c.StatusCode(500)
    //    };
}