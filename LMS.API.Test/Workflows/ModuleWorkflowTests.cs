using Xunit;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;

namespace LMS.API.Test.Workflows
{
    public class ModuleWorkflowTests
    {
        [Fact]
        public void ResolveCourse_ReturnsUnauthorized_WhenCallerIsNull()
        {
            var result = ModuleWorkflow.ResolveCourse(
                caller: null,
                requestedCourseId: 1,
                courseExists: true);

            var una = Assert.IsType<WorkflowResult<int>.Unauthorized>(result);
            Assert.Equal("Not authenticated.", una.Reason);
        }

        [Fact]
        public void ResolveCourse_ReturnsBadRequest_WhenRoleInvalid()
        {
            var caller = new CallerContext("u", false, false, null);

            var result = ModuleWorkflow.ResolveCourse(
                caller: caller,
                requestedCourseId: 5,
                courseExists: true);

            var bad = Assert.IsType<WorkflowResult<int>.BadRequest>(result);
            Assert.Equal("Invalid role.", bad.Reason);
        }

        [Fact]
        public void ResolveCourse_ReturnsBadRequest_WhenStudentHasNoCourse()
        {
            var caller = new CallerContext("s", false, true, null);

            var result = ModuleWorkflow.ResolveCourse(
                caller: caller,
                requestedCourseId: 10,
                courseExists: true);

            var bad = Assert.IsType<WorkflowResult<int>.BadRequest>(result);
            Assert.Equal("Invalid role.", bad.Reason);
        }

        [Fact]
        public void ResolveCourse_ReturnsBadRequest_WhenCourseDoesNotExist()
        {
            var caller = new CallerContext("t", true, false, null);

            var result = ModuleWorkflow.ResolveCourse(
                caller: caller,
                requestedCourseId: 99,
                courseExists: false);

            var bad = Assert.IsType<WorkflowResult<int>.BadRequest>(result);
            Assert.Equal("Invalid CourseId.", bad.Reason);
        }

        [Fact]
        public void ResolveCourse_ReturnsOk_ForTeacher()
        {
            var caller = new CallerContext("t", true, false, null);
            var requested = 42;

            var result = ModuleWorkflow.ResolveCourse(
                caller: caller,
                requestedCourseId: requested,
                courseExists: true);

            var ok = Assert.IsType<WorkflowResult<int>.Ok>(result);
            Assert.Equal(requested, ok.Value);
        }

        [Fact]
        public void ResolveCourse_ReturnsOk_ForStudent()
        {
            var caller = new CallerContext("s", false, true, 7);

            var result = ModuleWorkflow.ResolveCourse(
                caller: caller,
                requestedCourseId: 999,
                courseExists: true);

            var ok = Assert.IsType<WorkflowResult<int>.Ok>(result);
            Assert.Equal(7, ok.Value);
        }
    }
}