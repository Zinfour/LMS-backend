using System;
using System.Collections.Generic;
using Xunit;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;

namespace LMS.API.Test.Workflows
{
    public class LoginWorkflowTests
    {
        [Fact]
        public void Execute_ReturnsUnauthorized_WhenUserMissing()
        {
            var input = new LoginInput(
                FoundUserId: null,
                Username: "no-user",
                PasswordMatches: true,
                Roles: Array.Empty<string>(),
                Profile: null);

            var result = LoginWorkflow.Execute(input);

            var una = Assert.IsType<WorkflowResult<LoginPayload>.Unauthorized>(result);
            Assert.Equal("User does not exist.", una.Reason);
        }

        [Fact]
        public void Execute_ReturnsUnauthorized_WhenPasswordInvalid()
        {
            var input = new LoginInput(
                FoundUserId: "u1",
                Username: "u1",
                PasswordMatches: false,
                Roles: Array.Empty<string>(),
                Profile: new UserProfile("u1", DateTime.UtcNow, DateTime.UtcNow, "u1@example.com", "First", "Last", null, 1));

            var result = LoginWorkflow.Execute(input);

            var una = Assert.IsType<WorkflowResult<LoginPayload>.Unauthorized>(result);
            Assert.Equal("Invalid password.", una.Reason);
        }

        [Fact]
        public void Execute_ReturnsUnauthorized_WhenProfileMissing()
        {
            var input = new LoginInput(
                FoundUserId: "u1",
                Username: "u1",
                PasswordMatches: true,
                Roles: Array.Empty<string>(),
                Profile: null);

            var result = LoginWorkflow.Execute(input);

            var una = Assert.IsType<WorkflowResult<LoginPayload>.Unauthorized>(result);
            Assert.Equal("User profile missing.", una.Reason);
        }

        [Fact]
        public void Execute_ReturnsOk_WithFirstRole()
        {
            var profile = new UserProfile("u1", DateTime.UtcNow, DateTime.UtcNow, "u1@example.com", "First", "Last", null, 2);
            var input = new LoginInput(
                FoundUserId: "u1",
                Username: "u1",
                PasswordMatches: true,
                Roles: new List<string> { "Teacher", "Admin" },
                Profile: profile);

            var result = LoginWorkflow.Execute(input);

            var ok = Assert.IsType<WorkflowResult<LoginPayload>.Ok>(result);
            Assert.Equal(profile, ok.Value.Profile);
            Assert.Equal("Teacher", ok.Value.Role);
        }

        [Fact]
        public void Execute_ReturnsOk_DefaultsToStudent_WhenNoRoles()
        {
            var profile = new UserProfile("u2", DateTime.UtcNow, DateTime.UtcNow, "u2@example.com", "First", "Last", null, 3);
            var input = new LoginInput(
                FoundUserId: "u2",
                Username: "u2",
                PasswordMatches: true,
                Roles: Array.Empty<string>(),
                Profile: profile);

            var result = LoginWorkflow.Execute(input);

            var ok = Assert.IsType<WorkflowResult<LoginPayload>.Ok>(result);
            Assert.Equal("Student", ok.Value.Role);
            Assert.Equal(profile, ok.Value.Profile);
        }
    }
}