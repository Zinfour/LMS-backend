using LMS.API.Core.Types;
using LMS.API.Core.Workflows;
using LMS.API.DTOs;
using LMS.API.DTOs.Auth;
using LMS.API.DTOs.Course;
using LMS.API.DTOs.Resource;
using LMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Shell;

public static class Mappers
{
    public static ResultModel ToResultModel(this LoginPayload p) => new()
    {
        Id = p.Profile.Id,
        CreatedAt = p.Profile.CreatedAt,
        UpdatedAt = p.Profile.UpdatedAt,
        Email = p.Profile.Email,
        FirstName = p.Profile.FirstName,
        LastName = p.Profile.LastName,
        Role = p.Role,
        ImageUrl = p.Profile.ImageUrl,
        CourseId = p.Profile.CourseId,
        // Token filled in by the shell after signing.
    };

    public static ResourceDto ToDto(this ResourceView v, int courseId = 0) => new()
    {
        Id = v.Id,
        CreatedAt = v.CreatedAt,
        UpdatedAt = v.UpdatedAt,
        CreatedByUserId = v.CreatedByUserId,
        UpdatedByUserId = v.UpdatedByUserId,
        URL = v.URL,
        ResourceType = v.ResourceType,
        Name = v.Name,
        Description = v.Description,
        CourseId = courseId
    };

    public static ResourceView ToResourceView(this Models.ActivityResource r) => new(
    r.Id, r.CreatedAt, r.UpdatedAt,
    r.CreatedByUserId, r.UpdatedByUserId,
    r.URL, Tools.ResourceTypeToString(r.ResourceType),
    r.Name, r.Description, 0);

    public static CourseDto ToCourseDto(this Models.Course c, string userId = null!) => new()
    {
        Id = c.Id,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        Name = c.Name,
        Description = c.Description,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        ImageURL = c.ImageURL,
        Resources = c.Resources.Select(r => r.ToResourceDto()).ToList(),
        Students = c.Users.Where(u => u.Roles.Any(r => r.Name == Role.Student))
        .Select(u => u.ToUserDto()).ToList(),
        Teacher = c.Users.Where(u => u.Roles.Any(r => r.Name == Role.Teacher))
        .Select(u => u.ToUserDto()).FirstOrDefault() ?? new UserDto(),
        Modules = c.Modules.Select(m => new ModuleDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            ImageURL = m.ImageURL,
            CourseId = m.CourseId,
            ActivitiesNumber = m.Activities.Count,
            ResourcesNumber = m.Resources.Count,
            NumberOfCompletedActivities = userId == null ? 0 : m.Activities.Where(a => a.CompletedUsers.Any(u => u.Id == userId)).ToList().Count,
            Order = userId == null ? 0 : c.Modules.Count(md => md.CourseId == m.CourseId && md.StartDate < m.StartDate),
            CurrentStatus = userId == null ? ModuleStatus.inProgress : Tools.calculateStatus(m, userId)
        }).ToList()
    };

    public static UserDto ToUserDto(this Models.ApplicationUser u) => new()
    {
        Id = u.Id,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt,
        Email = u.Email ?? "",
        FirstName = u.FirstName,
        LastName = u.LastName,
        Role = u.Roles.Select(r => r.Name).FirstOrDefault() ?? "",
        ImageUrl = u.ImageUrl,
        CourseId = u.CourseId
    };

    // ---------- Resource projections ----------
    public static ResourceDto ToResourceDto(this CourseResource r) => new()
    {
        Id = r.Id,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        CreatedByUserId = r.CreatedByUserId,
        UpdatedByUserId = r.UpdatedByUserId,
        URL = r.URL,
        ResourceType = Tools.ResourceTypeToString(r.ResourceType),
        Name = r.Name,
        Description = r.Description,
        CourseId = r.CourseId
    };

    public static ResourceDto ToResourceDto(this ModuleResource r) => new()
    {
        Id = r.Id,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        CreatedByUserId = r.CreatedByUserId,
        UpdatedByUserId = r.UpdatedByUserId,
        URL = r.URL,
        ResourceType = Tools.ResourceTypeToString(r.ResourceType),
        Name = r.Name,
        Description = r.Description
    };

    public static ResourceDto ToResourceDto(this ActivityResource r) => new()
    {
        Id = r.Id,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        CreatedByUserId = r.CreatedByUserId,
        UpdatedByUserId = r.UpdatedByUserId,
        URL = r.URL,
        ResourceType = Tools.ResourceTypeToString(r.ResourceType),
        Name = r.Name,
        Description = r.Description
    };

    // ---------- Assignment projection (pure) ----------
    public static AssignmentDto? ToAssignmentDto(this Assignment? a) =>
        a is null ? null : new AssignmentDto
        {
            Id = a.Id,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            Title = a.Title,
            Description = a.Description,
            Deadline = a.Deadline,
            ActivityId = a.ActivityId,
            Submissions = a.Submissions.Select(s => new SubmissionDto
            {
                Id = s.Id,
                CreatedAt = s.CreatedAt,
                Text = s.Text,
                StudentId = s.StudentId,
                AssignmentId = s.AssignmentId
            }).ToList()
        };

    // ---------- Activity projection ----------
    public static ActivityDto ToActivityDto(this Activity a) => new()
    {
        Id = a.Id,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt,
        Type = a.Type.ToString(),
        Name = a.Name,
        StartTime = a.StartTime,
        EndTime = a.EndTime,
        Description = a.Description,
        ImageURL = a.ImageURL,
        ModuleId = a.ModuleId,
        Assignment = a.Assignment?.ToAssignmentDto(),
        Resources = a.Resources.Select(r => r.ToResourceDto()).ToList()
    };

    // ---------- Module projections ----------
    public static ModuleFullDto ToModuleFullDto(this Module m, int totalNumberOfModules) => new()
    {
        Id = m.Id,
        CreatedAt = m.CreatedAt,
        UpdatedAt = m.UpdatedAt,
        Name = m.Name,
        Description = m.Description,
        StartDate = m.StartDate,
        EndDate = m.EndDate,
        ImageURL = m.ImageURL,
        CourseId = m.CourseId,
        Activities = m.Activities.Select(a => a.ToActivityDto()).ToList(),
        Resources = m.Resources.Select(r => r.ToResourceDto()).ToList(),
        TotalNumberOfModules = totalNumberOfModules,
    };
}