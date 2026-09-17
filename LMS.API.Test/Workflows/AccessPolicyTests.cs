using Xunit;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;

namespace LMS.API.Test.Workflows
{
    public class AccessPolicyTests
    {
        [Fact]
        public void DenyIfNotAuthenticated_ReturnsUnauthorized_WhenCallerNull()
        {
            var result = AccessPolicy.DenyIfNotAuthenticated<string>(null);

            var unauthorized = Assert.IsType<WorkflowResult<string>.Unauthorized>(result);
            Assert.Equal("Not authenticated.", unauthorized.Reason);
        }

        [Fact]
        public void DenyIfNotAuthenticated_ReturnsNull_WhenCallerPresent()
        {
            var caller = new CallerContext("user1", false, false, null);

            var result = AccessPolicy.DenyIfNotAuthenticated<string>(caller);

            Assert.Null(result);
        }

        [Fact]
        public void DenyIfInvalidRole_ReturnsBadRequest_WhenRoleInvalid()
        {
            var caller = new CallerContext("user1", false, false, null);

            var result = AccessPolicy.DenyIfInvalidRole<string>(caller);

            var bad = Assert.IsType<WorkflowResult<string>.BadRequest>(result);
            Assert.Equal("Invalid role.", bad.Reason);
        }

        [Fact]
        public void DenyIfInvalidRole_ReturnsNull_WhenTeacherOrStudent()
        {
            var teacher = new CallerContext("t1", true, false, null);
            var student = new CallerContext("s1", false, true, null);

            Assert.Null(AccessPolicy.DenyIfInvalidRole<string>(teacher));
            Assert.Null(AccessPolicy.DenyIfInvalidRole<string>(student));
        }

        [Fact]
        public void DenyIfNotSelfOrTeacher_ReturnsForbidden_WhenNotTeacherAndDifferentUser()
        {
            var caller = new CallerContext("a", false, false, null);

            var result = AccessPolicy.DenyIfNotSelfOrTeacher<string>(caller, "other");

            var forb = Assert.IsType<WorkflowResult<string>.Forbidden>(result);
            Assert.Equal("Not allowed to act on behalf of another user.", forb.Reason);
        }

        [Fact]
        public void DenyIfNotSelfOrTeacher_ReturnsNull_WhenTeacher()
        {
            var caller = new CallerContext("t", true, false, null);

            Assert.Null(AccessPolicy.DenyIfNotSelfOrTeacher<string>(caller, "other"));
        }

        [Fact]
        public void DenyIfNotSelfOrTeacher_ReturnsNull_WhenSelf()
        {
            var caller = new CallerContext("me", false, false, null);

            Assert.Null(AccessPolicy.DenyIfNotSelfOrTeacher<string>(caller, "me"));
        }
    }
}