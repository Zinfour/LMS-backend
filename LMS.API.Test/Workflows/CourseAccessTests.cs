using Xunit;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;

namespace LMS.API.Test.Workflows
{
    public class CourseAccessTests
    {
        [Fact]
        public void ResolveCourseId_ReturnsRequestedCourseId_WhenCallerIsTeacher()
        {
            var teacher = new CallerContext("t", true, false, null);

            var result = CourseAccess.ResolveCourseId(teacher, 42);

            Assert.Equal(42, result);
        }

        [Fact]
        public void ResolveCourseId_ReturnsCallerCourseId_WhenCallerIsStudent()
        {
            var student = new CallerContext("s", false, true, 7);

            var result = CourseAccess.ResolveCourseId(student, 42);

            Assert.Equal(7, result);
        }

        [Fact]
        public void ResolveCourseId_ReturnsNull_WhenCallerIsNeitherTeacherNorStudent()
        {
            var caller = new CallerContext("u", false, false, null);

            var result = CourseAccess.ResolveCourseId(caller, 5);

            Assert.Null(result);
        }
    }
}