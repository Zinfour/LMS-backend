namespace LMS.API.Core.Types;

// Used by resource write workflows that need a caller + target parent.
public record ResourceWriteContext(
    CallerContext Caller,
    bool ParentExists,
    int ParentId,
    ResourceWrite Write);

// Used by user update workflow.
public record UserUpdateInput(
    CallerContext Caller,
    string TargetUserId,
    bool UserExists,
    bool CourseExists,
    string NewRole,
    string? NewPassword,
    IReadOnlyList<string> CurrentRoles);

// Used by user delete workflow.
public record UserDeleteInput(
    CallerContext Caller,
    string TargetUserId,
    bool UserExists);

// Used by module write workflows (create/update/delete stubs plus reads).
public record ModuleReadContext(
    CallerContext Caller,
    int ResolvedCourseId,
    bool CourseExists,
    bool ModuleExists);

// Identity of the caller, already resolved by the shell.
public record CallerContext(string UserId, bool IsTeacher, bool IsStudent, int? CourseId);

// A pure input for the "login" workflow, produced by the shell from UserManager.
public record LoginInput(
    string? FoundUserId,
    string Username,
    bool PasswordMatches,
    IReadOnlyList<string> Roles,
    UserProfile? Profile);

public record UserProfile(
    string Id, DateTime CreatedAt, DateTime UpdatedAt,
    string Email, string FirstName, string LastName,
    string? ImageUrl, int CourseId);

// A pure input for "complete activity".
public record CompleteActivityInput(
    CallerContext Caller,
    string TargetUserId,
    int ActivityId,
    bool UserExists,
    bool ActivityExists,
    bool AlreadyCompleted);

// Course read model (already in memory, projected by the shell).
public record CourseView(
    int Id, DateTime CreatedAt, DateTime UpdatedAt,
    string Name, string Description,
    DateOnly StartDate, DateOnly EndDate,
    string? ImageURL,
    IReadOnlyList<ResourceView> Resources,
    IReadOnlyList<ModuleView> Modules,
    IReadOnlyList<UserView> Students,
    UserView? Teacher);

public record ModuleView(
    int Id, string Name, string Description,
    DateOnly StartDate, DateOnly EndDate, string? ImageURL,
    int CourseId, int ActivitiesNumber, int ResourcesNumber,
    int NumberOfCompletedActivities, int Order,
    ModuleStatus CurrentStatus);

public enum ModuleStatus { completed, overdue, inProgress, locked }

public record ResourceView(
    int Id, DateTime CreatedAt, DateTime UpdatedAt,
    string CreatedByUserId, string? UpdatedByUserId,
    string? URL, string ResourceType, string Name, string Description,
    int CourseId);

public record UserView(
    string Id, DateTime CreatedAt, DateTime UpdatedAt,
    string Email, string FirstName, string LastName,
    string Role, string? ImageUrl, int CourseId);

// Pure input for Create/Update Course.
public record CourseWrite(
    string Name, string Description,
    DateOnly StartDate, DateOnly EndDate,
    string? ImageURL);

// Pure input for Create/Update Resource.
public record ResourceWrite(
    string? URL, string ResourceType, string Name, string Description);

public record ActivityContext(
    CallerContext Caller,
    bool ParentModuleExists,
    int ParentModuleId,
    DateOnly ModuleStartDate,
    DateOnly ModuleEndDate,
    IReadOnlyList<ActivityWrite>? ExistingActivities,
    ActivityWrite Write);

public record ActivityWrite(
    int Id, string Name, string Description, DateTime StartTime, DateTime EndTime, bool ActivityExists);

// Pure input for creating a submission.
public record SubmissionWrite(
    string Text, int AssignmentId);

public record AssignmentMeta(int AssignmentId, DateTime Deadline);

public record NewSubmission(
    string Text, string StudentId, int AssignmentId,
    bool Overdue, DateTime CreatedAt);