using System;
using System.Collections.Generic;
using Xunit;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;
using LMS.API.Shell;

namespace LMS.API.Test.Workflows
{
    public class UserWorkflowTests
    {
        [Fact]
        public void PlanUpdate_ReturnsForbidden_WhenCallerNotTeacher()
        {
            var caller = new CallerContext("c1", false, false, null);

            var input = new UserUpdateInput(
                Caller: caller,
                TargetUserId: "u1",
                UserExists: true,
                CourseExists: true,
                NewRole: "Student",
                NewPassword: null,
                CurrentRoles: Array.Empty<string>());

            var result = UserWorkflow.PlanUpdate(input);

            var forb = Assert.IsType<WorkflowResult<UserUpdatePlan>.Forbidden>(result);
            Assert.Equal("Only teachers may update users.", forb.Reason);
        }

        [Fact]
        public void PlanUpdate_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var caller = new CallerContext("t1", true, false, null);

            var input = new UserUpdateInput(
                Caller: caller,
                TargetUserId: "missing",
                UserExists: false,
                CourseExists: true,
                NewRole: "Student",
                NewPassword: null,
                CurrentRoles: Array.Empty<string>());

            var result = UserWorkflow.PlanUpdate(input);

            var nf = Assert.IsType<WorkflowResult<UserUpdatePlan>.NotFound>(result);
            Assert.Equal("User not found.", nf.Reason);
        }

        [Fact]
        public void PlanUpdate_ReturnsBadRequest_WhenCourseInvalid()
        {
            var caller = new CallerContext("t1", true, false, null);

            var input = new UserUpdateInput(
                Caller: caller,
                TargetUserId: "u1",
                UserExists: true,
                CourseExists: false,
                NewRole: "Student",
                NewPassword: null,
                CurrentRoles: Array.Empty<string>());

            var result = UserWorkflow.PlanUpdate(input);

            var bad = Assert.IsType<WorkflowResult<UserUpdatePlan>.BadRequest>(result);
            Assert.Equal("Invalid CourseId.", bad.Reason);
        }

        [Fact]
        public void PlanUpdate_ReturnsBadRequest_WhenRoleInvalid()
        {
            var caller = new CallerContext("t1", true, false, null);

            var input = new UserUpdateInput(
                Caller: caller,
                TargetUserId: "u1",
                UserExists: true,
                CourseExists: true,
                NewRole: "Admin",
                NewPassword: null,
                CurrentRoles: Array.Empty<string>());

            var result = UserWorkflow.PlanUpdate(input);

            var bad = Assert.IsType<WorkflowResult<UserUpdatePlan>.BadRequest>(result);
            Assert.Equal("Invalid role: Admin.", bad.Reason);
        }

        [Fact]
        public void PlanUpdate_ReturnsOk_ChangePasswordTrue_WhenNewPasswordProvided()
        {
            var caller = new CallerContext("t1", true, false, null);

            var input = new UserUpdateInput(
                Caller: caller,
                TargetUserId: "u1",
                UserExists: true,
                CourseExists: true,
                NewRole: "Student",
                NewPassword: "newpass",
                CurrentRoles: new[] { "Student" });

            var result = UserWorkflow.PlanUpdate(input);

            var ok = Assert.IsType<WorkflowResult<UserUpdatePlan>.Ok>(result);
            Assert.True(ok.Value.ChangePassword);
            Assert.False(ok.Value.ChangeRole);
        }

        [Fact]
        public void PlanUpdate_ReturnsOk_ChangeRoleTrue_WhenRoleDiffers()
        {
            var caller = new CallerContext("t1", true, false, null);

            var input = new UserUpdateInput(
                Caller: caller,
                TargetUserId: "u1",
                UserExists: true,
                CourseExists: true,
                NewRole: "Teacher",
                NewPassword: null,
                CurrentRoles: new[] { "Student" });

            var result = UserWorkflow.PlanUpdate(input);

            var ok = Assert.IsType<WorkflowResult<UserUpdatePlan>.Ok>(result);
            Assert.False(ok.Value.ChangePassword);
            Assert.True(ok.Value.ChangeRole);
        }
    }
}