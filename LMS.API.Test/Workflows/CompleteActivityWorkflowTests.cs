using Xunit;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;

namespace LMS.API.Test.Workflows
{
    public class CompleteActivityWorkflowTests
    {
        [Fact]
        public void Execute_ReturnsBadRequest_WhenRoleInvalid()
        {
            var input = new CompleteActivityInput(
                Caller: new CallerContext("user1", false, false, null),
                TargetUserId: "user1",
                ActivityId: 1,
                UserExists: true,
                ActivityExists: true,
                AlreadyCompleted: false);

            var result = CompleteActivityWorkflow.Validate(input);

            var bad = Assert.IsType<WorkflowResult<Unit>.BadRequest>(result);
            Assert.Equal("Invalid role.", bad.Reason);
        }

        [Fact]
        public void Execute_ReturnsForbidden_WhenNotSelfAndNotTeacher()
        {
            var input = new CompleteActivityInput(
                Caller: new CallerContext("caller", false, true, null),
                TargetUserId: "other",
                ActivityId: 1,
                UserExists: true,
                ActivityExists: true,
                AlreadyCompleted: false);

            var result = CompleteActivityWorkflow.Validate(input);

            var forb = Assert.IsType<WorkflowResult<Unit>.Forbidden>(result);
            Assert.Equal("Not allowed to act on behalf of another user.", forb.Reason);
        }

        [Fact]
        public void Execute_ReturnsNotFound_WhenUserMissing()
        {
            var input = new CompleteActivityInput(
                Caller: new CallerContext("t", true, false, null),
                TargetUserId: "missing-user",
                ActivityId: 2,
                UserExists: false,
                ActivityExists: true,
                AlreadyCompleted: false);

            var result = CompleteActivityWorkflow.Validate(input);

            var nf = Assert.IsType<WorkflowResult<Unit>.NotFound>(result);
            Assert.Equal($"Couldn't find user with id: {input.TargetUserId}.", nf.Reason);
        }

        [Fact]
        public void Execute_ReturnsNotFound_WhenActivityMissing()
        {
            var input = new CompleteActivityInput(
                Caller: new CallerContext("t", true, false, null),
                TargetUserId: "some-user",
                ActivityId: 99,
                UserExists: true,
                ActivityExists: false,
                AlreadyCompleted: false);

            var result = CompleteActivityWorkflow.Validate(input);

            var nf = Assert.IsType<WorkflowResult<Unit>.NotFound>(result);
            Assert.Equal($"Couldn't find activity with id: {input.ActivityId}.", nf.Reason);
        }

        [Fact]
        public void Execute_ReturnsOk_WhenAlreadyCompleted()
        {
            var input = new CompleteActivityInput(
                Caller: new CallerContext("s", false, true, null),
                TargetUserId: "s",
                ActivityId: 5,
                UserExists: true,
                ActivityExists: true,
                AlreadyCompleted: true);

            var result = CompleteActivityWorkflow.Validate(input);

            Assert.IsType<WorkflowResult<Unit>.Ok>(result);
        }

        [Fact]
        public void Execute_ReturnsOk_WhenNotCompleted()
        {
            var input = new CompleteActivityInput(
                Caller: new CallerContext("s", false, true, null),
                TargetUserId: "s",
                ActivityId: 5,
                UserExists: true,
                ActivityExists: true,
                AlreadyCompleted: false);

            var result = CompleteActivityWorkflow.Validate(input);

            Assert.IsType<WorkflowResult<Unit>.Ok>(result);
        }
    }
}