using System;
using System.Collections.Generic;
using System.Linq;
using LMS.API;
using LMS.API.Core.Types;
using LMS.API.Core.Workflows;
using LMS.API.DTOs;
using LMS.API.DTOs.Auth;
using LMS.API.DTOs.Resource;
using LMS.API.Models;
using LMS.API.Models.Resources;
using LMS.API.Shell;
using Xunit;

namespace LMS.API.Test
{
    public class MappersTests
    {
        [Fact]
        public void ToResultModel_MapsProfileFields()
        {
            var profile = new UserProfile(
                "user-1",
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow,
                "me@example.com",
                "First",
                "Last",
                "http://img",
                5);

            var payload = new LoginPayload(profile, "Teacher");

            var result = payload.ToResultModel();

            Assert.Equal(profile.Id, result.Id);
            Assert.Equal(profile.CreatedAt, result.CreatedAt);
            Assert.Equal(profile.UpdatedAt, result.UpdatedAt);
            Assert.Equal(profile.Email, result.Email);
            Assert.Equal(profile.FirstName, result.FirstName);
            Assert.Equal(profile.LastName, result.LastName);
            Assert.Equal("Teacher", result.Role);
            Assert.Equal(profile.ImageUrl, result.ImageUrl);
            Assert.Equal(profile.CourseId, result.CourseId);
        }

        [Fact]
        public void CourseResource_ToResourceDto_MapsFields()
        {
            var now = DateTime.UtcNow;
            var r = new CourseResource
            {
                Id = 10,
                CreatedAt = now.AddHours(-1),
                UpdatedAt = now,
                CreatedByUserId = "creator",
                UpdatedByUserId = "updater",
                URL = "http://res",
                ResourceType = ResourceType.Link,
                Name = "ResName",
                Description = "Desc",
                CourseId = 99
            };

            var dto = r.ToResourceDto();

            Assert.Equal(r.Id, dto.Id);
            Assert.Equal(r.CreatedAt, dto.CreatedAt);
            Assert.Equal(r.UpdatedAt, dto.UpdatedAt);
            Assert.Equal(r.CreatedByUserId, dto.CreatedByUserId);
            Assert.Equal(r.UpdatedByUserId, dto.UpdatedByUserId);
            Assert.Equal(r.URL, dto.URL);
            Assert.Equal(Tools.ResourceTypeToString(r.ResourceType), dto.ResourceType);
            Assert.Equal(r.Name, dto.Name);
            Assert.Equal(r.Description, dto.Description);
            Assert.Equal(r.CourseId, dto.CourseId);
        }

        [Fact]
        public void ModuleResource_ToResourceDto_SetsDefaultCourseId()
        {
            var now = DateTime.UtcNow;
            var r = new ModuleResource
            {
                Id = 11,
                CreatedAt = now.AddHours(-2),
                UpdatedAt = now,
                CreatedByUserId = "c",
                UpdatedByUserId = "u",
                URL = "http://mod",
                ResourceType = ResourceType.Instruction,
                Name = "MName",
                Description = "MDesc",
                ModuleId = 5
            };

            var dto = r.ToResourceDto();

            Assert.Equal(r.Id, dto.Id);
            Assert.Equal(Tools.ResourceTypeToString(r.ResourceType), dto.ResourceType);
            // ModuleResource -> ResourceDto mapping does not set CourseId, default is 0
            Assert.Equal(0, dto.CourseId);
        }

        [Fact]
        public void ActivityResource_ToResourceDto_And_ToResourceView_MapsFields()
        {
            var now = DateTime.UtcNow;
            var ar = new ActivityResource
            {
                Id = 20,
                CreatedAt = now.AddMinutes(-5),
                UpdatedAt = now,
                CreatedByUserId = "au",
                UpdatedByUserId = "uu",
                URL = "http://act",
                ResourceType = ResourceType.Summary,
                Name = "AName",
                Description = "ADesc",
                ActivityId = 7,
            };

            var dto = ar.ToResourceDto();
            Assert.Equal(ar.Id, dto.Id);
            Assert.Equal(Tools.ResourceTypeToString(ar.ResourceType), dto.ResourceType);
            Assert.Equal(ar.Name, dto.Name);
            Assert.Equal(ar.Description, dto.Description);
            // ActivityResource mapping to ResourceDto does not set CourseId
            Assert.Equal(0, dto.CourseId);

            var view = ar.ToResourceView();
            Assert.Equal(ar.Id, view.Id);
            Assert.Equal(ar.CreatedAt, view.CreatedAt);
            Assert.Equal(ar.UpdatedAt, view.UpdatedAt);
            Assert.Equal(ar.CreatedByUserId, view.CreatedByUserId);
            Assert.Equal(ar.UpdatedByUserId, view.UpdatedByUserId);
            Assert.Equal(ar.URL, view.URL);
            Assert.Equal(Tools.ResourceTypeToString(ar.ResourceType), view.ResourceType);
            Assert.Equal(ar.Name, view.Name);
            Assert.Equal(ar.Description, view.Description);
            // ResourceView produced from ActivityResource uses CourseId = 0
            Assert.Equal(0, view.CourseId);
        }

        [Fact]
        public void ToAssignmentDto_ReturnsNull_ForNullInput()
        {
            Assignment? a = null;
            var dto = a.ToAssignmentDto();
            Assert.Null(dto);
        }

        [Fact]
        public void ToAssignmentDto_MapsSubmissions()
        {
            var now = DateTime.UtcNow;
            var assignment = new Assignment
            {
                Id = 30,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-1),
                Title = "Ass",
                Description = "Desc",
                Deadline = now.AddDays(1),
                ActivityId = 2,
                Submissions = new List<Submission>
                {
                    new Submission { Id = 1, CreatedAt = now.AddHours(-2), Text = "S1", StudentId = "stu1", AssignmentId = 30 },
                    new Submission { Id = 2, CreatedAt = now.AddHours(-1), Text = "S2", StudentId = "stu2", AssignmentId = 30 }
                }
            };

            var dto = assignment.ToAssignmentDto();
            Assert.NotNull(dto);
            Assert.Equal(assignment.Id, dto!.Id);
            Assert.Equal(2, dto.Submissions.Count);
            Assert.Contains(dto.Submissions, s => s.Text == "S1" && s.StudentId == "stu1");
            Assert.Contains(dto.Submissions, s => s.Text == "S2" && s.StudentId == "stu2");
        }

        [Fact]
        public void ToActivityDto_MapsAssignmentAndResources()
        {
            var now = DateTime.UtcNow;
            var activity = new Activity
            {
                Id = 40,
                CreatedAt = now.AddHours(-10),
                UpdatedAt = now.AddHours(-5),
                Type = ActivityType.Assignment,
                Name = "ActName",
                StartTime = now,
                EndTime = now.AddHours(1),
                Description = "ActDesc",
                ImageURL = "http://img/activity",
                ModuleId = 3,
                Assignment = new Assignment
                {
                    Id = 50,
                    CreatedAt = now.AddDays(-2),
                    UpdatedAt = now.AddDays(-1),
                    Title = "A",
                    Description = "ADesc",
                    Deadline = now.AddDays(2),
                    ActivityId = 40,
                    Submissions = new List<Submission>()
                },
                Resources = new List<ActivityResource>
                {
                    new ActivityResource { Id = 101, CreatedAt = now, UpdatedAt = now, CreatedByUserId = "u", ResourceType = ResourceType.TextMaterial, Name = "R1", Description = "D1" }
                }
            };

            var dto = activity.ToActivityDto();

            Assert.Equal(activity.Id, dto.Id);
            Assert.Equal(activity.Name, dto.Name);
            Assert.Equal(activity.Type.ToString(), dto.Type);
            Assert.NotNull(dto.Assignment);
            Assert.Equal(activity.Assignment.Id, dto.Assignment!.Id);
            Assert.Single(dto.Resources);
            Assert.Equal(activity.Resources[0].Id, dto.Resources[0].Id);
            Assert.Equal(Tools.ResourceTypeToString(activity.Resources[0].ResourceType), dto.Resources[0].ResourceType);
        }

        [Fact]
        public void ToUserDto_UsesFirstRoleNameOrEmpty()
        {
            var user = new ApplicationUser
            {
                Id = "user-x",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow,
                Email = "u@x.com",
                FirstName = "Fn",
                LastName = "Ln",
                ImageUrl = "http://img",
                CourseId = 2,
                Roles = new List<ApplicationRole> { new ApplicationRole("Teacher") }
            };

            var dto = user.ToUserDto();

            Assert.Equal(user.Id, dto.Id);
            Assert.Equal(user.Email, dto.Email);
            Assert.Equal("Teacher", dto.Role);
            Assert.Equal(user.CourseId, dto.CourseId);
        }
    }
}