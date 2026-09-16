using System.Security.Claims;
using LMS.API.Core.Types;
using LMS.API.Models;

namespace LMS.API.Shell;

public static class CurrentUser
{
    public static CallerContext? From(ClaimsPrincipal principal, int? courseIdFromDb = null)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null) return null;

        return new CallerContext(
            UserId: id,
            IsTeacher: principal.IsInRole(Role.Teacher),
            IsStudent: principal.IsInRole(Role.Student),
            CourseId: courseIdFromDb);
    }
}