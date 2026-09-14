using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class CourseAccess
{
    public static int? ResolveCourseId(CallerContext caller, int requestedCourseId)
    {
        if (caller.IsTeacher) return requestedCourseId;
        if (caller.IsStudent) return caller.CourseId;
        return null;
    }
}