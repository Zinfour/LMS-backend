using LMS.API.Data;
using LMS.API.Models;
using LMS.API.Models.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MovieApi.Data.Seeding;

public static class DbSeeder
{
    private const string Password = "Test123!";

    // Index 0 => student@gmail.com, index 1 => student1@gmail.com, etc.
    private static readonly (string FirstName, string LastName)[] StudentNames =
    [
        ("Liam", "Nguyen"),
        ("Ava", "Johansson"),
        ("Noah", "Becker"),
        ("Emma", "Rossi"),
        ("Oliver", "Kowalski"),
        ("Sophia", "Kim"),
        ("Lucas", "Silva"),
        ("Mia", "Andersen"),
        ("Ethan", "Novak"),
        ("Isabella", "Moreau"),
    ];

    private static readonly (string FirstName, string LastName)[] UnassignedStudentNames =
    [
        ("Mateo", "Garcia"),
        ("Grace", "Williams"),
        ("Henry", "Patel"),
        ("Chloe", "Martin"),
        ("Daniel", "Brown"),
        ("Nora", "Taylor"),
        ("Leo", "Wilson"),
        ("Zoe", "Clark"),
        ("Samuel", "Davis"),
        ("Layla", "Moore"),
    ];

    private static readonly (string FirstName, string LastName)[] UnassignedTeacherNames =
    [
        ("Priya", "Shah"),
        ("Marcus", "Reed"),
        ("Elena", "Petrov"),
        ("David", "Okafor"),
    ];

    private static readonly string[] AvatarUrls =
    [
        "https://images.unsplash.com/photo-1654110455429-cf322b40a906?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8YXZhdGFyfGVufDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8YXZhdGFyfGVufDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1740252117044-2af197eea287?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NHx8YXZhdGFyfGVufDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1701615004837-40d8573b6652?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NXx8YXZhdGFyfGVufDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1527980965255-d3b416303d12?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Nnx8YXZhdGFyfGVufDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1740252117027-4275d3f84385?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OHx8YXZhdGFyfGVufDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1740252117012-bb53ad05e370?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTJ8fGF2YXRhcnxlbnwwfDJ8MHx8fDI%3D",
        "https://images.unsplash.com/photo-1569779213435-ba3167dde7cc?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTd8fGF2YXRhcnxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1605087880595-8cc6db61f3c6?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NjZ8fGF2YXRhcnxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1620889406270-03d743d544c2?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Nzd8fGF2YXRhcnxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1633245976565-3084baca3c31?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Nzl8fGF2YXRhcnxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1560096434-c1cec8609cc5?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OTF8fGF2YXRhcnxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1645107914156-fc9e45906b04?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OTZ8fGF2YXRhcnxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1529068755536-a5ade0dcb4e8?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OTR8fGF2YXRhcnxlbnwwfDJ8MHx8fDI%3D",
        "https://images.unsplash.com/photo-1623577284502-d65cdc6ba0b6?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTh8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1645107914072-6f16b732f224?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NjN8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1654762699761-b6d13143bb2e?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NjJ8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1695013079138-d39ea65ab0b6?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Nzh8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1759701546655-d90ec831aa52?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8ODF8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1727933959587-fd3cf7926475?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NzZ8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1602494518375-c2dc5376f522?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8ODd8fHByb2ZpbGUlMjBwaWN0dXJlfDB8MnwwfHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1655293459479-cacd56abeaf6?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTExfHxwcm9maWxlJTIwcGljdHVyZXxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1586299485759-f62264d6b63f?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTEyfHxwcm9maWxlJTIwcGljdHVyZXxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1625474407059-8c543b6d1fc6?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTA5fHxwcm9maWxlJTIwcGljdHVyZXxlbnwwfDJ8MHx8Mg%3D%3D",
        "https://images.unsplash.com/photo-1650381473833-3e2c74a40fbf?w=200&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTE2fHxwcm9maWxlJTIwcGljdHVyZXxlbnwwfDJ8MHx8Mg%3D%3D",
    ];

    private static readonly string[] SubmissionResponses =
    [
        "I approached this task by first outlining the expected behavior, then implementing the smallest working version before adding validation and handling the edge cases I found during testing. The final solution keeps the code readable and documents the assumptions I made along the way.",
        "For this assignment, I broke the problem into smaller components and tested each one before wiring them together. I refined the types after discovering a few inconsistent states, and the submitted version includes the final flow along with the reasoning behind the important implementation choices.",
        "I started with the requirements and created a simple plan for the data flow before writing the UI and supporting logic. After testing the main scenario and error cases, I adjusted the implementation to avoid duplicated work and made the final result easier to maintain.",
        "My solution focuses on a clear structure: reusable functions handle the repeated behavior, while the main component coordinates the user-facing workflow. I also verified that the application behaves predictably with incomplete input and that the output matches the acceptance criteria.",
        "I implemented the requested feature incrementally and used the intermediate results to identify where the state could become out of sync. The final submission includes defensive checks, clear naming, and a completed implementation that covers both the normal path and likely edge cases.",
    ];

    // How many activities (in module order, out of Module 1/2/3/4) each student has completed so far.
    // Modules 5 and 6 haven't started yet, so nobody has activity there.
    private static readonly int[][] StudentProgress =
    [
        [6, 7, 5, 5], // student@gmail.com  - on track, currently working through the routing module
        [6, 7, 6, 5], // student1@gmail.com - ahead of schedule
        [6, 7, 5, 5], // student2@gmail.com - on track
        [6, 6, 3, 0], // student3@gmail.com - fell behind during module 2
        [6, 7, 1, 0], // student4@gmail.com - barely started module 3
        [4, 0, 0, 0], // student5@gmail.com - struggling since module 1
        [6, 7, 6, 6], // student6@gmail.com - finished everything released so far
        [6, 7, 4, 5], // student7@gmail.com - average pace
        [6, 7, 6, 5], // student8@gmail.com - caught back up on module 3
        [6, 4, 0, 0], // student9@gmail.com - behind since module 2
    ];

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var context = services.GetRequiredService<LmsContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var roleName in new[] { Role.Teacher, Role.Student })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole(roleName));
                if (!roleResult.Succeeded)
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }

        var course = await context.Course.FirstOrDefaultAsync(cancellationToken);
        var courseWasCreated = course == null;

        if (courseWasCreated)
        {
            course = BuildCourse();
            context.Course.Add(course);
            await context.SaveChangesAsync(cancellationToken);
        }

        var teacher = await CreateUserAsync(userManager, "teacher@gmail.com", "Jordan", "Blake", Role.Teacher, course!.Id, AvatarUrls[0]);

        var students = new List<ApplicationUser>();
        var emailIdx = 0;
        for (var i = 0; i < StudentNames.Length; i++)
        {
            var email = emailIdx == 0 ? "student@gmail.com" : $"student{emailIdx}@gmail.com";
            var (firstName, lastName) = StudentNames[i];
            students.Add(await CreateUserAsync(userManager, email, firstName, lastName, Role.Student, course.Id, AvatarUrls[i + 1]));
            emailIdx++;
        }

        for (var i = 0; i < UnassignedStudentNames.Length; i++)
        {
            var (firstName, lastName) = UnassignedStudentNames[i];
            await CreateUserAsync(userManager, $"student{emailIdx}@gmail.com", firstName, lastName, Role.Student, null, AvatarUrls[i + 11]);
            emailIdx++;
        }

        for (var i = 0; i < UnassignedTeacherNames.Length; i++)
        {
            var (firstName, lastName) = UnassignedTeacherNames[i];
            await CreateUserAsync(userManager, $"teacher{i + 2}@gmail.com", firstName, lastName, Role.Teacher, null, AvatarUrls[i + 21]);
        }

        await context.SaveChangesAsync(cancellationToken);

        if (courseWasCreated)
        {
            AddResources(course!, teacher.Id);
            await context.SaveChangesAsync(cancellationToken);

            SeedStudentProgress(context, course!, students);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string firstName,
        string lastName,
        string role,
        int? courseId,
        string? imageUrl)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            existing.ImageUrl ??= imageUrl;
            return existing;
        }

        var now = DateTime.UtcNow;
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            ImageUrl = imageUrl,
            CreatedAt = now,
            UpdatedAt = now,
            CourseId = courseId,
        };

        var createResult = await userManager.CreateAsync(user, Password);
        if (!createResult.Succeeded)
            throw new Exception($"Failed to create user '{email}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

        var roleResult = await userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
            throw new Exception($"Failed to assign role '{role}' to '{email}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

        return user;
    }

    /// <summary>
    /// Marks activities as completed (and records assignment submissions) for each student, based on
    /// <see cref="StudentProgress"/>. Activities are marked complete in module/activity order, so a student
    /// with a progress count of 5 for a module has completed the first 5 activities of that module.
    /// </summary>
    private static void SeedStudentProgress(LmsContext context, Course course, List<ApplicationUser> students)
    {
        var now = DateTime.UtcNow;
        var random = new Random(42); // fixed seed so the seed data is reproducible between runs

        for (var s = 0; s < students.Count; s++)
        {
            var student = students[s];
            var completedCounts = StudentProgress[s];

            for (var m = 0; m < completedCounts.Length; m++)
            {
                var module = course.Modules[m];
                var completedCount = completedCounts[m];

                for (var a = 0; a < completedCount && a < module.Activities.Count; a++)
                {
                    var activity = module.Activities[a];
                    activity.CompletedUsers.Add(student);

                    if (activity.Assignment != null)
                    {
                        context.Submission.Add(new Submission
                        {
                            CreatedAt = now,
                            Text = SubmissionResponses[random.Next(SubmissionResponses.Length)],
                            StudentId = student.Id,
                            AssignmentId = activity.Assignment.Id,
                        });
                    }
                }
            }
        }
    }

    private static void AddResources(Course course, string teacherId)
    {
        var now = DateTime.UtcNow;

        course.Resources.Add(new CourseResource
        {
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Instruction,
            Name = "Course Syllabus",
            Description = "Overview of the course structure, weekly pacing, grading criteria, and learning objectives.",
            URL = null
        });
        course.Resources.Add(new CourseResource
        {
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Reference,
            Name = "React Documentation",
            Description = "The official React documentation, used as the primary reference throughout the course.",
            URL = "https://react.dev/learn"
        });
        course.Resources.Add(new CourseResource
        {
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.TextMaterial,
            Name = "Frontend Style Guide",
            Description = "The style guide students are expected to follow for all React components submitted in assignments.",
            URL = "https://github.com/airbnb/javascript/tree/master/react"
        });

        var modules = course.Modules;

        AddModuleResources(modules[0], teacherId,
            ("TypeScript Handbook", ResourceType.Reference, "https://www.typescriptlang.org/docs/handbook/intro.html"),
            ("Vite Guide", ResourceType.Reference, "https://vitejs.dev/guide/"));

        AddModuleResources(modules[1], teacherId,
            ("React Docs: Describing the UI", ResourceType.Reference, "https://react.dev/learn/describing-the-ui"),
            ("Thinking in React", ResourceType.Summary, "https://react.dev/learn/thinking-in-react"));

        AddModuleResources(modules[2], teacherId,
            ("TanStack Query Docs", ResourceType.Reference, "https://tanstack.com/query/latest"),
            ("useReducer Reference", ResourceType.Reference, "https://react.dev/reference/react/useReducer"));

        AddModuleResources(modules[3], teacherId,
            ("React Router Docs", ResourceType.Reference, "https://reactrouter.com/"),
            ("Vite: Env Variables and Modes", ResourceType.Reference, "https://vitejs.dev/guide/env-and-mode"));

        AddModuleResources(modules[4], teacherId,
            ("Vitest Docs", ResourceType.Reference, "https://vitest.dev/"),
            ("Testing Library Docs", ResourceType.Reference, "https://testing-library.com/docs/react-testing-library/intro/"));

        AddModuleResources(modules[5], teacherId,
            ("Vite: Building for Production", ResourceType.Reference, "https://vitejs.dev/guide/build.html"),
            ("GitHub Actions Docs", ResourceType.Reference, "https://docs.github.com/actions"));

        AddActivityResources(course, teacherId);
    }

    private static void AddModuleResources(Module module, string teacherId, params (string Name, ResourceType Type, string Url)[] resources)
    {
        var now = DateTime.UtcNow;

        foreach (var (name, type, url) in resources)
        {
            module.Resources.Add(new ModuleResource
            {
                CreatedAt = now,
                UpdatedAt = now,
                CreatedByUserId = teacherId,
                ResourceType = type,
                Name = name,
                Description = $"Reading material for the \"{module.Name}\" module.",
                URL = url
            });
        }
    }

    private static Course BuildCourse()
    {
        var now = DateTime.UtcNow;

        return new Course
        {
            CreatedAt = now,
            UpdatedAt = now,
            Name = "Modern Frontend Engineering with React, TypeScript & Vite",
            Description = "A hands-on course covering component-driven UI development with React and TypeScript, powered by Vite. Students design, build, test, and ship a production-grade single-page application.",
            StartDate = new DateOnly(2026, 7, 6),
            EndDate = new DateOnly(2026, 12, 18),
            ImageURL = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
            Modules =
            [
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Toolchain Setup & TypeScript Foundations",
                    Description = "Getting the development environment running and learning the TypeScript fundamentals used throughout the course.",
                    StartDate = new DateOnly(2026, 7, 6),
                    EndDate = new DateOnly(2026, 7, 26),
                    ImageURL = "https://picsum.photos/seed/typescript-foundations/800/450",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Kickoff: Course Overview & Dev Environment Setup",
                            StartTime = new DateTime(2026, 7, 6, 10, 0, 0),
                            EndTime = new DateTime(2026, 7, 6, 12, 0, 0),
                            Description = "Course structure, grading, and getting Node, Git, and your editor set up."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "TypeScript Basics: Types, Interfaces & Generics",
                            StartTime = new DateTime(2026, 7, 7, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 10, 23, 59, 0),
                            Description = "Self-paced lesson covering primitive types, interfaces, unions, and generics."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Configuring Vite + TypeScript From Scratch",
                            StartTime = new DateTime(2026, 7, 11, 13, 0, 0),
                            EndTime = new DateTime(2026, 7, 11, 15, 0, 0),
                            Description = "Hands-on workshop scaffolding a Vite + React + TypeScript project from an empty folder."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "tsconfig.json Deep Dive & Strict Mode",
                            StartTime = new DateTime(2026, 7, 13, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 16, 23, 59, 0),
                            Description = "Understanding compiler options and why strict mode matters for larger codebases."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Build a Typed Utility Library",
                            StartTime = new DateTime(2026, 7, 14, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 23, 23, 59, 0),
                            Description = "Write a small collection of strongly-typed utility functions, complete with generics and unit tests.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Build a Typed Utility Library",
                                Description = "Implement and export at least five reusable, fully-typed utility functions.",
                                Deadline = new DateTime(2026, 7, 23, 23, 59, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Code Review: TypeScript Patterns & Pitfalls",
                            StartTime = new DateTime(2026, 7, 24, 10, 0, 0),
                            EndTime = new DateTime(2026, 7, 24, 11, 30, 0),
                            Description = "Group walkthrough of common TypeScript mistakes found in the utility library submissions."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "React Fundamentals & Component Architecture",
                    Description = "Core React concepts: components, props, state, and structuring a UI as a component tree.",
                    StartDate = new DateOnly(2026, 7, 27),
                    EndDate = new DateOnly(2026, 8, 16),
                    ImageURL = "https://picsum.photos/seed/react-fundamentals/800/450",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "JSX & Functional Components",
                            StartTime = new DateTime(2026, 7, 27, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 29, 23, 59, 0),
                            Description = "Writing your first components with JSX and understanding how React renders them."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Props, State & the Component Tree",
                            StartTime = new DateTime(2026, 7, 30, 10, 0, 0),
                            EndTime = new DateTime(2026, 7, 30, 12, 0, 0),
                            Description = "Passing data down with props, managing local state, and composing components."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Building a Reusable Button & Card Library",
                            StartTime = new DateTime(2026, 7, 31, 13, 0, 0),
                            EndTime = new DateTime(2026, 7, 31, 15, 0, 0),
                            Description = "Workshop building a small library of reusable, prop-driven UI components."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "React Hooks: useState & useEffect In Depth",
                            StartTime = new DateTime(2026, 8, 3, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 6, 23, 59, 0),
                            Description = "Managing state and side effects with the two most commonly used hooks."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Refactoring Class Components to Hooks",
                            StartTime = new DateTime(2026, 8, 7, 13, 0, 0),
                            EndTime = new DateTime(2026, 8, 7, 15, 0, 0),
                            Description = "Hands-on refactor of legacy class components into functional components with hooks."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Build a Product Catalog UI",
                            StartTime = new DateTime(2026, 8, 8, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 15, 23, 59, 0),
                            Description = "Build a product listing page from a provided component library and static dataset.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Build a Product Catalog UI",
                                Description = "Render a filterable grid of products using reusable components and props.",
                                Deadline = new DateTime(2026, 8, 15, 23, 59, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Other,
                            Name = "Peer Code Review Session",
                            StartTime = new DateTime(2026, 8, 16, 10, 0, 0),
                            EndTime = new DateTime(2026, 8, 16, 11, 0, 0),
                            Description = "Students review each other's product catalog submissions in small groups."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "State Management & Data Fetching",
                    Description = "Managing complex state and fetching remote data the React way.",
                    StartDate = new DateOnly(2026, 8, 17),
                    EndDate = new DateOnly(2026, 9, 6),
                    ImageURL = "https://picsum.photos/seed/state-management/800/450",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Context API & useReducer",
                            StartTime = new DateTime(2026, 8, 17, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 20, 23, 59, 0),
                            Description = "Sharing state across components without prop drilling."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Comparing State Approaches: Context vs Zustand vs Redux",
                            StartTime = new DateTime(2026, 8, 21, 10, 0, 0),
                            EndTime = new DateTime(2026, 8, 21, 12, 0, 0),
                            Description = "Trade-offs between built-in and third-party state management solutions."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Implementing a Shopping Cart with useReducer",
                            StartTime = new DateTime(2026, 8, 24, 13, 0, 0),
                            EndTime = new DateTime(2026, 8, 24, 15, 0, 0),
                            Description = "Workshop building cart add/remove/update logic with useReducer."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Data Fetching Patterns & React Query Fundamentals",
                            StartTime = new DateTime(2026, 8, 26, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 29, 23, 59, 0),
                            Description = "Fetching, caching, and synchronizing server data with TanStack Query."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Build a Paginated Product List with React Query",
                            StartTime = new DateTime(2026, 8, 27, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 5, 23, 59, 0),
                            Description = "Fetch a paginated product list from an API and cache it with React Query.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Build a Paginated Product List with React Query",
                                Description = "Implement pagination, caching, and refetching using useQuery.",
                                Deadline = new DateTime(2026, 9, 5, 23, 59, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Handling Loading & Error States Workshop",
                            StartTime = new DateTime(2026, 9, 3, 13, 0, 0),
                            EndTime = new DateTime(2026, 9, 3, 15, 0, 0),
                            Description = "Patterns for skeleton loaders, retries, and user-friendly error messages."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Routing, Vite Tooling & Performance",
                    Description = "Multi-page navigation, Vite configuration, and keeping the app fast.",
                    StartDate = new DateOnly(2026, 9, 7),
                    EndDate = new DateOnly(2026, 9, 27),
                    ImageURL = "https://picsum.photos/seed/routing-vite/800/450",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Client-Side Routing with React Router",
                            StartTime = new DateTime(2026, 9, 7, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 8, 23, 59, 0),
                            Description = "Setting up routes, links, and route parameters with React Router."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Nested Routes, Layouts & Protected Routes",
                            StartTime = new DateTime(2026, 9, 9, 10, 0, 0),
                            EndTime = new DateTime(2026, 9, 9, 12, 0, 0),
                            Description = "Composing layouts with nested routes and gating pages behind authentication."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Code Splitting & Lazy Loading with Vite",
                            StartTime = new DateTime(2026, 9, 11, 13, 0, 0),
                            EndTime = new DateTime(2026, 9, 11, 15, 0, 0),
                            Description = "Reducing initial bundle size with route-based code splitting."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Vite Plugins & Environment Variables",
                            StartTime = new DateTime(2026, 9, 12, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 15, 23, 59, 0),
                            Description = "Configuring environment-specific builds and extending Vite with plugins."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Add Authentication-Aware Routing",
                            StartTime = new DateTime(2026, 9, 13, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 25, 23, 59, 0),
                            Description = "Add protected routes that redirect unauthenticated users to the login page.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Add Authentication-Aware Routing",
                                Description = "Implement route guards and redirect logic based on authentication state.",
                                Deadline = new DateTime(2026, 9, 25, 23, 59, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Performance Profiling with React DevTools",
                            StartTime = new DateTime(2026, 9, 20, 13, 0, 0),
                            EndTime = new DateTime(2026, 9, 20, 15, 0, 0),
                            Description = "Finding and fixing unnecessary re-renders using the React DevTools profiler."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Testing & Quality Assurance",
                    Description = "Writing automated tests to keep the application reliable as it grows.",
                    StartDate = new DateOnly(2026, 9, 28),
                    EndDate = new DateOnly(2026, 10, 18),
                    ImageURL = "https://picsum.photos/seed/testing-qa/800/450",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Unit Testing Fundamentals with Vitest",
                            StartTime = new DateTime(2026, 9, 28, 9, 0, 0),
                            EndTime = new DateTime(2026, 10, 1, 23, 59, 0),
                            Description = "Writing and running unit tests for plain functions and hooks with Vitest."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Component Testing with React Testing Library",
                            StartTime = new DateTime(2026, 10, 2, 10, 0, 0),
                            EndTime = new DateTime(2026, 10, 2, 12, 0, 0),
                            Description = "Testing components the way users interact with them, not their implementation details."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Writing Tests for the Shopping Cart Feature",
                            StartTime = new DateTime(2026, 10, 5, 13, 0, 0),
                            EndTime = new DateTime(2026, 10, 5, 15, 0, 0),
                            Description = "Workshop covering tests for reducers, components, and edge cases in the cart feature."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Achieve 80% Test Coverage on Core Components",
                            StartTime = new DateTime(2026, 10, 6, 9, 0, 0),
                            EndTime = new DateTime(2026, 10, 16, 23, 59, 0),
                            Description = "Add unit and component tests until coverage of the core components reaches 80%.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Achieve 80% Test Coverage on Core Components",
                                Description = "Submit a coverage report showing at least 80% coverage on the core components.",
                                Deadline = new DateTime(2026, 10, 16, 23, 59, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Other,
                            Name = "Intro to End-to-End Testing with Playwright",
                            StartTime = new DateTime(2026, 10, 17, 10, 0, 0),
                            EndTime = new DateTime(2026, 10, 17, 11, 30, 0),
                            Description = "A first look at simulating full user journeys in a real browser."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Deployment, CI/CD & Capstone Project",
                    Description = "Shipping the application to production and wrapping up with a capstone project.",
                    StartDate = new DateOnly(2026, 10, 19),
                    EndDate = new DateOnly(2026, 12, 14),
                    ImageURL = "https://picsum.photos/seed/deployment-capstone/800/450",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Production Builds & Environment Configs in Vite",
                            StartTime = new DateTime(2026, 10, 19, 9, 0, 0),
                            EndTime = new DateTime(2026, 10, 22, 23, 59, 0),
                            Description = "Optimizing and configuring production builds for different deployment targets."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "CI/CD Pipelines with GitHub Actions",
                            StartTime = new DateTime(2026, 10, 23, 10, 0, 0),
                            EndTime = new DateTime(2026, 10, 23, 12, 0, 0),
                            Description = "Automating linting, tests, and deployments with GitHub Actions workflows."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Deploying to Vercel & Netlify",
                            StartTime = new DateTime(2026, 10, 26, 13, 0, 0),
                            EndTime = new DateTime(2026, 10, 26, 15, 0, 0),
                            Description = "Hands-on deployment of the class project to two popular hosting platforms."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Capstone Project: Build & Ship a Full Application",
                            StartTime = new DateTime(2026, 10, 27, 9, 0, 0),
                            EndTime = new DateTime(2026, 12, 10, 23, 59, 0),
                            Description = "Design, build, test, and deploy a complete React + TypeScript + Vite application.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Capstone Project: Build & Ship a Full Application",
                                Description = "A complete application combining routing, state management, data fetching, and tests, deployed live.",
                                Deadline = new DateTime(2026, 12, 10, 23, 59, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Other,
                            Name = "Final Presentations & Demo Day",
                            StartTime = new DateTime(2026, 12, 14, 10, 0, 0),
                            EndTime = new DateTime(2026, 12, 14, 16, 0, 0),
                            Description = "Students present their capstone projects to the class."
                        }
                    ]
                }
            ]
        };
    }
    
    private static void AddActivityResources(Course course, string teacherId)
    {
        var now = DateTime.UtcNow;

        var resourcesByActivity =
            new Dictionary<string, (ResourceType Type, string Name, string Description, string? Url)[]>
            {
                // ============================================================
                // MODULE 1
                // Non-assignments: TextMaterial only
                // Assignment: Instruction (+ TextMaterial)
                // ============================================================

                ["Kickoff: Course Overview & Dev Environment Setup"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Getting Started",
                        """
                        Before beginning the technical exercises in this course, make sure your development environment is working correctly. Install Node.js and Git, choose an editor such as Visual Studio Code, and make sure you can open a terminal and run commands from your project directory. You should also configure Git with your name and email so that commits are associated with the correct identity.
                        Create a small test project and verify that Node and npm are available from the command line. Initialize a Git repository, create an initial commit, and confirm that your editor can open the project folder. The purpose of this activity is not to build an application yet, but to remove the environment problems that can otherwise interrupt later exercises.
                        When you finish, you should have a working development environment, a functioning Git installation, and a basic understanding of where your project files live and how you will run development commands throughout the course.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Node.js Documentation",
                        "Use the Node.js documentation when checking your installation or looking up npm commands.",
                        "https://nodejs.org/"
                    ),
                    (
                        ResourceType.Link,
                        "Git Documentation",
                        "Reference for Git initialization, configuration, commits, and everyday commands.",
                        "https://git-scm.com/docs"
                    )
                ],

                ["TypeScript Basics: Types, Interfaces & Generics"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "TypeScript Foundations",
                        """
                        TypeScript adds a static type system to JavaScript, allowing you to describe the kinds of values your application expects to work with. Start by reviewing primitive types such as string, number, and boolean, then move into arrays, objects, unions, and literal types. Pay particular attention to the difference between a value having a particular runtime shape and TypeScript being able to prove that shape at compile time.
                        Interfaces and type aliases allow you to give meaningful names to object structures. This becomes especially important in React applications because components frequently receive structured data through props. A well-defined type can document what a component expects while also helping the compiler catch mistakes before the application runs.
                        Generics take this idea further by allowing you to write reusable code without giving up type information. Instead of accepting completely unknown values, a generic function can preserve the relationship between its inputs and outputs. As you work through this activity, focus on understanding that relationship rather than memorizing generic syntax.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "TypeScript Handbook",
                        "Primary reference for the language features introduced in this activity.",
                        "https://www.typescriptlang.org/docs/handbook/intro.html"
                    ),
                    (
                        ResourceType.Link,
                        "TypeScript Types Reference",
                        "A compact reference covering common TypeScript types, narrowing, and generic patterns.",
                        "https://example.com/typescript/types-reference.pdf"
                    )
                ],

                ["Configuring Vite + TypeScript From Scratch"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Workshop Material",
                        """
                        Vite is designed to give frontend projects a fast development workflow while keeping the underlying project structure understandable. During this workshop, pay attention to the relationship between the package manifest, the entry point, the HTML document, the TypeScript configuration, and the Vite configuration itself.
                        When you scaffold the project, inspect the generated files instead of treating the command as a black box. Identify which file starts the React application, where dependencies are declared, which scripts launch development and production commands, and how Vite discovers the source files it needs to process.
                        The important outcome is not simply having a project that starts successfully. You should understand enough of the generated structure that you can open an unfamiliar Vite project and recognize the major pieces immediately.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vite Guide",
                        "Official guide covering project creation, development, configuration, and builds.",
                        "https://vitejs.dev/guide/"
                    ),
                    (
                        ResourceType.Link,
                        "Starter Project Archive",
                        "Example starter project that you can compare against after completing the workshop.",
                        "https://example.com/vite/starter-project.zip"
                    )
                ],

                ["tsconfig.json Deep Dive & Strict Mode"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Understanding Strict TypeScript",
                        """
                        The TypeScript compiler does much more than decide whether a file contains valid TypeScript syntax. Its configuration determines how aggressively the compiler checks your code and what assumptions it is allowed to make about values. The strict family of options is especially important in application development because it exposes incorrect assumptions early, before they become runtime bugs.
                        One of the most important examples is strictNullChecks. With this option enabled, TypeScript makes you acknowledge that a value may be null or undefined when the type says that it can be. This often feels inconvenient at first, but it encourages you to handle states explicitly instead of accidentally calling a method on a missing value.
                        Spend time reading the compiler options in your tsconfig.json and changing a few settings experimentally. The objective is not to memorize every compiler option. Instead, develop an intuition for how compiler configuration affects the safety, maintainability, and developer experience of a TypeScript project.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "TypeScript Compiler Options",
                        "Official reference for TypeScript compiler configuration.",
                        "https://www.typescriptlang.org/tsconfig/"
                    ),
                    (
                        ResourceType.Link,
                        "Example Strict Configuration",
                        "Example tsconfig configuration for comparing strict compiler settings.",
                        "https://example.com/typescript/strict-tsconfig.json"
                    )
                ],

                ["Build a Typed Utility Library"] =
                [
                    (
                        ResourceType.Instruction,
                        "Assignment Instructions",
                        """
                        In this assignment, you will build a small library of reusable TypeScript utility functions. Implement at least five utilities and design each one so that it can be reused with different input values rather than being tied to one specific example. At least some of your utilities should make meaningful use of generic types.
                        Before implementing each function, think about its contract. What values can it receive? What should it return? Is there a relationship between the type of the input and the type of the result that TypeScript should preserve? Your goal is to communicate those relationships through the type system instead of weakening the implementation with any or unnecessary type assertions.
                        Add unit tests for the utilities and include both normal cases and useful edge cases. Your finished library should be easy for another developer to import, understand, and use without needing to inspect the implementation to discover what the API is supposed to do.
                        """,
                        null
                    ),
                    (
                        ResourceType.TextMaterial,
                        "Designing Reusable Generic Utilities",
                        """
                        Generic functions are most useful when they preserve information rather than throwing it away. For example, if a function accepts a value of type T and returns that same value, the type parameter allows TypeScript to preserve the specific type chosen by the caller. The function remains reusable while still providing precise type checking.
                        As you design your utilities, look for relationships between values. A transformation from one type to another may require two generic parameters, while a function operating on objects may benefit from a constraint. The important question is always what information the compiler needs in order to understand the function's behavior.
                        Good utility code is also deliberate about edge cases. Think about empty arrays, missing values, duplicate items, invalid input, and other cases that could produce surprising behavior. Strong typing helps prevent many mistakes, but it does not replace thoughtful API design.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "TypeScript Generics",
                        "Official documentation for generic functions, constraints, and reusable types.",
                        "https://www.typescriptlang.org/docs/handbook/2/generics.html"
                    ),
                    (
                        ResourceType.Link,
                        "Utility Types",
                        "Reference for built-in TypeScript utility types that may be useful in the assignment.",
                        "https://www.typescriptlang.org/docs/handbook/utility-types.html"
                    ),
                    (
                        ResourceType.Link,
                        "Assignment Starter ZIP",
                        "Optional example project structure for the utility library assignment.",
                        "https://example.com/assignments/typed-utility-library.zip"
                    )
                ],

                ["Code Review: TypeScript Patterns & Pitfalls"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Preparing for Code Review",
                        """
                        Before the review session, read through your utility library as if you were seeing it for the first time. Identify one place where your type definitions are particularly strong, one place where the implementation could be simplified, and one decision that another developer might question.
                        During the review, focus on concrete observations rather than general statements such as "the code looks good." Explain why a type definition helps, why a generic constraint is necessary, or why a particular implementation could become difficult to maintain. You should also be prepared to receive suggestions about naming, structure, testing, and readability.
                        The purpose of this activity is to practice treating code as something that other developers must understand and modify. A technically correct implementation can still be difficult to work with if its intent is unclear.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Everyday TypeScript",
                        "Reference material for common TypeScript patterns, narrowing, and type design.",
                        "https://www.typescriptlang.org/docs/handbook/2/everyday-types.html"
                    )
                ],

                // ============================================================
                // MODULE 2
                // ============================================================

                ["JSX & Functional Components"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Understanding JSX",
                        """
                        JSX allows you to describe a user interface using a syntax that looks similar to HTML while still being part of JavaScript. A JSX element can contain expressions, receive props, and compose other components, which means that it should be thought of as a way of describing a tree of UI rather than as a separate templating language.
                        Functional components are ordinary JavaScript or TypeScript functions that return UI. A component can accept inputs through props and can compose smaller components to create a larger interface. As you work through this activity, focus on the idea that a component should have a clear responsibility and a predictable relationship between its inputs and rendered output.
                        Pay attention to the difference between JavaScript expressions and JSX syntax. You should become comfortable embedding values inside JSX, rendering lists, using conditional expressions, and passing data into child components before moving into more advanced React features.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Writing Markup with JSX",
                        "Official React guide to JSX syntax and writing UI markup.",
                        "https://react.dev/learn/writing-markup-with-jsx"
                    ),
                    (
                        ResourceType.Link,
                        "JSX Quick Reference",
                        "Example PDF covering common JSX syntax and component patterns.",
                        "https://example.com/react/jsx-cheat-sheet.pdf"
                    )
                ],

                ["Props, State & the Component Tree"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Thinking About Component Boundaries",
                        """
                        React applications are easier to maintain when components have clear responsibilities. A component that tries to manage data fetching, business rules, layout, and every small visual detail at once becomes difficult to test and reuse. At the same time, splitting every small element into its own component can make the code harder to follow.
                        A useful starting point is to identify pieces of the interface that have their own behavior, data requirements, or reusable presentation. Then decide which components own state and which components simply receive data. This creates an explicit flow of information from parents to children.
                        The best component hierarchy is not necessarily the one with the most components. It is the one that makes the application's responsibilities and data flow easy to understand.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Passing Props",
                        "Official React guide covering data passed from parent components to children.",
                        "https://react.dev/learn/passing-props-to-a-component"
                    ),
                    (
                        ResourceType.Link,
                        "Thinking in React",
                        "Walkthrough for identifying component boundaries and application data flow.",
                        "https://react.dev/learn/thinking-in-react"
                    ),
                    (
                        ResourceType.Link,
                        "Component Architecture Guide",
                        "Example component-tree diagram and architecture notes.",
                        "https://example.com/react/component-architecture.pdf"
                    )
                ],

                ["Building a Reusable Button & Card Library"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Designing Reusable Components",
                        """
                        Reusable components work best when their public API expresses meaningful variations rather than exposing every implementation detail. A Button should allow callers to provide the information that actually changes, such as its label, disabled state, or action, while keeping its internal structure consistent.
                        The same principle applies to a Card. Decide which parts of the content should arrive as props and which structural decisions belong inside the component. Try using the component in several different contexts to discover whether its API is genuinely reusable or accidentally tied to the first screen where you built it.
                        Avoid adding props simply because they are technically possible. A small, clear component API is often more reusable than an extremely flexible component with dozens of configuration options.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Components",
                        "Reference material for building and composing reusable React components.",
                        "https://react.dev/learn/your-first-component"
                    )
                ],

                ["React Hooks: useState & useEffect In Depth"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "State and Effects",
                        """
                        React state represents information that can change over time and affect what the component renders. When state changes, React schedules another render so the component can produce updated UI. The important design question is not simply where to put useState, but which values genuinely need to be state and which values can be calculated from existing props or state.
                        Effects solve a different problem. An effect is useful when your component needs to synchronize with something outside React, such as a browser API, a subscription, a timer, or an external data source. An effect should not be used merely because you need to calculate a value from existing state. Derived values can usually be calculated directly during rendering.
                        Pay close attention to effect dependencies and cleanup. A dependency array is not a performance trick; it describes which reactive values the effect depends on. Cleanup functions are equally important when the effect creates subscriptions, timers, event listeners, or other resources that need to be released.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "useState Reference",
                        "Official reference for managing local component state.",
                        "https://react.dev/reference/react/useState"
                    ),
                    (
                        ResourceType.Link,
                        "useEffect Reference",
                        "Official reference explaining effects and synchronization with external systems.",
                        "https://react.dev/reference/react/useEffect"
                    ),
                    (
                        ResourceType.Link,
                        "Hooks Examples ZIP",
                        "Example exercises demonstrating common hook patterns.",
                        "https://example.com/react/hooks-examples.zip"
                    )
                ],

                ["Refactoring Class Components to Hooks"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Refactoring to Hooks",
                        """
                        Converting a class component to a functional component is an opportunity to understand what the original implementation was actually responsible for. Before changing the code, identify its state, lifecycle behavior, event handlers, subscriptions, and cleanup logic.
                        Hooks do not provide a one-to-one translation for every class lifecycle method. In particular, an effect should represent synchronization with an external system rather than being used as a generic replacement for every lifecycle callback. This distinction helps prevent effects from becoming a collection of unrelated logic.
                        Once the refactor is complete, compare the old and new implementations from the user's perspective. The goal is to change the implementation model while preserving the important behavior of the original component.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React API Reference",
                        "Reference for hooks and the functional component model.",
                        "https://react.dev/reference/react"
                    ),
                    (
                        ResourceType.Link,
                        "Refactoring Example",
                        "Example before-and-after comparison of a class component and a hooks-based implementation.",
                        "https://example.com/react/class-to-hooks.pdf"
                    )
                ],

                ["Build a Product Catalog UI"] =
                [
                    (
                        ResourceType.Instruction,
                        "Assignment Instructions",
                        """
                        Build a product catalog interface that displays a collection of products in a reusable grid. Each product should be rendered through components that receive their data through props rather than reaching directly into a global data structure. The page should support filtering and should update the visible list when the selected filter changes.
                        Before you begin coding, sketch the component hierarchy and decide where filter state should live. Avoid storing values that can be calculated from the existing product data and selected filter. Your components should also account for an empty result state so that the interface remains understandable when a filter matches no products.
                        Use the supplied dataset and concentrate on component design, prop usage, and predictable state flow. By the end of the assignment, you should have a small but complete UI that demonstrates the fundamental React architecture covered in this module.
                        """,
                        null
                    ),
                    (
                        ResourceType.TextMaterial,
                        "Planning the Catalog",
                        """
                        A filterable catalog is a useful example of how React state and derived data work together. The product dataset itself does not need to become state if it never changes during the user's interaction. Instead, the selected filter can be stored as state and the visible product list can be calculated from the dataset and that selected value.
                        Keep your component boundaries focused. A product card should primarily be concerned with presenting one product, while the catalog page can coordinate filtering and the list of products. This separation makes the components easier to understand and gives you reusable pieces that can be used elsewhere later.
                        As you work, think about what happens when there are no products, when the user changes the filter repeatedly, and when additional fields are added to a product. Good component APIs make these changes easier to accommodate.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Passing Props",
                        "Use this when deciding how product data should move through your component tree.",
                        "https://react.dev/learn/passing-props-to-a-component"
                    ),
                    (
                        ResourceType.Link,
                        "Conditional Rendering",
                        "Useful reference for empty states and conditional sections of the catalog.",
                        "https://react.dev/learn/conditional-rendering"
                    ),
                    (
                        ResourceType.Link,
                        "Product Dataset ZIP",
                        "Example product dataset and optional starter files.",
                        "https://example.com/assignments/product-catalog-dataset.zip"
                    ),
                    (
                        ResourceType.Link,
                        "Catalog Requirements PDF",
                        "Example functional requirements and visual acceptance criteria.",
                        "https://example.com/assignments/product-catalog-requirements.pdf"
                    )
                ],

                ["Peer Code Review Session"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Peer Review Guide",
                        """
                        During this session, you will review another student's product catalog implementation and provide constructive technical feedback. Begin by understanding the application before suggesting changes. Trace how product data enters the component tree, where filter state is stored, and how the visible product list is derived.
                        Look specifically for opportunities to improve component boundaries, naming, prop design, accessibility, and readability. Avoid rewriting the project simply because you would personally structure it differently. A good review identifies a concrete problem, explains why it matters, and proposes a practical improvement.
                        Finish the review by identifying at least one strength in the implementation. The purpose of this activity is not only to find mistakes but to learn how other developers approach the same problem and to become more comfortable discussing technical decisions.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Thinking in React",
                        "Use this as a reference when evaluating component structure and data flow.",
                        "https://react.dev/learn/thinking-in-react"
                    )
                ],

                // ============================================================
                // MODULE 3
                // ============================================================

                ["Context API & useReducer"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Shared State with Context and Reducers",
                        """
                        Context and useReducer solve different problems but can work particularly well together. A reducer gives you a structured way to describe how state changes in response to actions, while context allows components deeper in the tree to access shared values without passing them through every intermediate component.
                        Reducers are especially useful when a piece of state has several related transitions. Instead of updating multiple pieces of state independently, actions such as add, remove, or update can describe what happened and a single reducer can determine the next state. This makes complex transitions easier to reason about and easier to test in isolation.
                        Context should still be introduced deliberately. Making everything global can make dependencies less obvious. Use it when multiple distant components genuinely need access to the same value, and keep local state local when a component or small subtree is the only consumer.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "useContext",
                        "Official reference for consuming context values in React.",
                        "https://react.dev/reference/react/useContext"
                    ),
                    (
                        ResourceType.Link,
                        "useReducer",
                        "Official reference for reducer-based state management.",
                        "https://react.dev/reference/react/useReducer"
                    ),
                    (
                        ResourceType.Link,
                        "Reducer Patterns",
                        "Example PDF containing reducer patterns and common implementation mistakes.",
                        "https://example.com/react/reducer-patterns.pdf"
                    )
                ],

                ["Comparing State Approaches: Context vs Zustand vs Redux"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Choosing a State Management Strategy",
                        """
                        State management decisions should be based on the kind of state you are managing rather than on popularity alone. Local UI state, shared client state, and server state have different characteristics and often benefit from different tools. React's built-in features may be enough for one category while a specialized library makes another significantly easier.
                        When comparing libraries, look at the mental model they introduce. A tool that reduces boilerplate but creates unfamiliar concepts can still be a poor fit for a team that rarely needs its advanced features. Likewise, a more structured solution may be worthwhile when predictable state transitions and debugging become important.
                        The goal of this activity is to practice making a technology choice based on requirements. There is rarely a single correct answer independent of the application.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Zustand Documentation",
                        "Reference documentation for Zustand.",
                        "https://zustand.docs.pmnd.rs/"
                    ),
                    (
                        ResourceType.Link,
                        "Redux Documentation",
                        "Reference documentation for Redux and Redux Toolkit.",
                        "https://redux.js.org/"
                    ),
                    (
                        ResourceType.Link,
                        "Comparison Worksheet",
                        "Example worksheet for comparing state management approaches.",
                        "https://example.com/state/state-management-comparison.pdf"
                    )
                ],

                ["Implementing a Shopping Cart with useReducer"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Designing Reducer-Based State",
                        """
                        A shopping cart is a useful example of state that has several related transitions. Instead of updating individual properties directly from different event handlers, you can describe user intent through actions such as add, remove, increment, and decrement and let the reducer determine the next state.
                        Reducers should remain pure and predictable. Given the same state and the same action, the reducer should produce the same result without changing the existing state object or performing unrelated side effects. This makes the logic easier to reason about and straightforward to test.
                        As you build the cart, think carefully about the state shape and the rules that apply when quantities reach their boundaries. Good state design should make those rules easy to express instead of forcing each UI component to duplicate the same business logic.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "useReducer Reference",
                        "Official documentation for implementing reducer-based state transitions.",
                        "https://react.dev/reference/react/useReducer"
                    ),
                    (
                        ResourceType.Link,
                        "Shopping Cart Starter ZIP",
                        "Example starting project containing products and basic components.",
                        "https://example.com/workshops/shopping-cart.zip"
                    )
                ],

                ["Data Fetching Patterns & React Query Fundamentals"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Understanding Server State",
                        """
                        Data retrieved from an API behaves differently from local UI state. Server data can become stale, may be shared by multiple components, can require refetching, and often needs to be represented with loading and error states. Managing these concerns manually in every component can quickly lead to duplicated logic.
                        TanStack Query provides a structured model for this kind of server state. Queries describe how data is fetched and identified, while the query cache allows multiple components to reuse previously retrieved results. The library can also coordinate refetching and invalidation so that the interface stays synchronized with the server.
                        As you learn the fundamentals, focus on the mental model rather than memorizing configuration options. Ask what data belongs in the server-state cache, what belongs in local component state, and which events should cause a query to refetch or become invalid.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "TanStack Query",
                        "Primary documentation for React Query and server-state management.",
                        "https://tanstack.com/query/latest"
                    ),
                    (
                        ResourceType.Link,
                        "Query Fundamentals",
                        "Example PDF covering queries, caching, loading states, and invalidation.",
                        "https://example.com/react-query/query-fundamentals.pdf"
                    ),
                    (
                        ResourceType.Link,
                        "Example Products API",
                        "Example API endpoint for practicing data fetching.",
                        "https://example.com/api/products"
                    )
                ],

                ["Build a Paginated Product List with React Query"] =
                [
                    (
                        ResourceType.Instruction,
                        "Assignment Instructions",
                        """
                        Build a product list that retrieves data from an API and divides the results into pages. Use TanStack Query to manage the remote data rather than manually storing the fetched results and request state in several local variables. Your interface should clearly communicate when data is loading, when a request fails, and when there are no results.
                        Design your query keys so that different pages can be cached independently and make sure changing the current page results in the correct query being requested. The pagination controls should prevent invalid navigation and should make it clear when the user is already on the first or last available page.
                        Before submitting, test the behavior when navigating between pages repeatedly. Verify that cached results behave as expected and that the application does not issue unnecessary requests simply because unrelated parts of the component rendered again.
                        """,
                        null
                    ),
                    (
                        ResourceType.TextMaterial,
                        "Thinking About Pagination",
                        """
                        Pagination is not only a UI problem; it also changes how your application identifies server data. If page one and page two contain different results, they must be represented by different query keys so that the cache can distinguish them.
                        Think carefully about what should happen when the user changes pages. The previous page may still be useful information and can sometimes remain available in the interface while the next page is loading. This can make the experience feel much smoother than replacing the entire screen with a loading indicator on every navigation.
                        Also consider failure scenarios. A request for one page can fail even when another page has already loaded successfully. Your UI should make it clear which operation failed and give the user an obvious way to recover.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Paginated Queries",
                        "Official TanStack Query guide covering pagination patterns.",
                        "https://tanstack.com/query/latest/docs/framework/react/guides/paginated-queries"
                    ),
                    (
                        ResourceType.Link,
                        "Query Keys",
                        "Reference for designing stable query keys that correctly identify cached data.",
                        "https://tanstack.com/query/latest/docs/framework/react/guides/query-keys"
                    ),
                    (
                        ResourceType.Link,
                        "Pagination API ZIP",
                        "Example mock API project for testing paginated requests locally.",
                        "https://example.com/assignments/paginated-products-api.zip"
                    ),
                    (
                        ResourceType.Link,
                        "Assignment Requirements",
                        "Example PDF containing the assignment rubric and acceptance criteria.",
                        "https://example.com/assignments/react-query-pagination.pdf"
                    ),
                    (
                        ResourceType.Link,
                        "Pagination Sandbox",
                        "Example endpoint for experimenting with page-based API requests.",
                        "https://example.com/api/pagination"
                    )
                ],

                ["Handling Loading & Error States Workshop"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Designing Asynchronous States",
                        """
                        A data-driven component can exist in several meaningful states, and those states should be represented explicitly in the user interface. An initial loading state means that the application does not yet have the information it needs. An empty successful result means the request worked but there is nothing to display. An error state means the application was unable to retrieve the expected data.
                        These situations should not collapse into one generic message. A loading indicator can provide immediate feedback, an empty state can explain what the absence of data means, and an error state can give the user a recovery path. Clear state distinctions make asynchronous interfaces much easier to understand.
                        Test each state deliberately. A component that looks perfect with a fast and successful network response can still feel broken when the connection is slow or the server returns an error.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Query Functions",
                        "Reference material for how TanStack Query handles successful and failed requests.",
                        "https://tanstack.com/query/latest/docs/framework/react/guides/query-functions"
                    )
                ],

                // ============================================================
                // MODULE 4
                // ============================================================

                ["Client-Side Routing with React Router"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Routing Fundamentals",
                        """
                        Client-side routing allows a single-page application to present different views based on the current URL without requiring a complete browser navigation for every transition. The URL becomes an important part of application state because it can identify which screen is being displayed and can sometimes contain parameters needed to load data.
                        Begin with simple routes and links, then introduce route parameters and nested routes. Pay attention to the difference between navigating through the application's router and loading a URL directly in the browser. Your application needs to handle both situations correctly.
                        A good routing structure should make the relationship between URLs and pages easy to understand. As the application grows, consistent route definitions and layouts help prevent navigation logic from becoming scattered throughout individual components.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Router",
                        "Official React Router documentation.",
                        "https://reactrouter.com/"
                    ),
                    (
                        ResourceType.Link,
                        "Routing Examples ZIP",
                        "Example collection of route configurations and parameterized routes.",
                        "https://example.com/react-router/routing-examples.zip"
                    )
                ],

                ["Nested Routes, Layouts & Protected Routes"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Designing Route Hierarchies",
                        """
                        Nested routes are useful because many pages share a common shell. Instead of repeating a sidebar, header, or navigation component in every page, a parent route can provide the shared layout while child routes render their page-specific content inside it.
                        Protected routes add another layer of application behavior because navigation now depends on authentication state. It is important to keep this behavior predictable: a user who does not have access should never briefly see protected content, and a user who successfully logs in should have a clear path back into the application.
                        Think of the router as part of your application's architecture rather than as a collection of links. Route structure can communicate which parts of the application belong together and which parts require particular conditions.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Router Tutorial",
                        "Walkthrough covering route configuration and navigation.",
                        "https://reactrouter.com/en/main/start/tutorial"
                    ),
                    (
                        ResourceType.Link,
                        "Nested Routes Reference",
                        "Reference material for nested layouts and route hierarchies.",
                        "https://reactrouter.com/start/framework/routing"
                    ),
                    (
                        ResourceType.Link,
                        "Authentication Flow",
                        "Example PDF illustrating login, redirect, and protected-route behavior.",
                        "https://example.com/react-router/auth-flow.pdf"
                    )
                ],

                ["Code Splitting & Lazy Loading with Vite"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Code Splitting and Lazy Loading",
                        """
                        A modern frontend build can contain a large amount of JavaScript even when an individual page only needs a small portion of it. Code splitting addresses this by allowing the application to load some modules later instead of sending the entire application to the browser immediately.
                        Route-based splitting is particularly useful because users do not necessarily need the code for every section of an application on the first page they visit. A dashboard, settings page, and reporting interface may all live in the same project while being loaded only when the user navigates to them.
                        Remember that performance improvements should be measured rather than assumed. Splitting too aggressively can create unnecessary network requests and make the application more complicated without producing a meaningful improvement. The goal is to reduce unnecessary initial work while keeping the application's behavior predictable.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Dynamic Imports",
                        "Reference for JavaScript dynamic imports, which are commonly used as the basis for code splitting.",
                        "https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/import"
                    ),
                    (
                        ResourceType.Link,
                        "Vite Build Guide",
                        "Reference for production builds and bundle generation.",
                        "https://vitejs.dev/guide/build.html"
                    ),
                    (
                        ResourceType.Link,
                        "Code Splitting Demo",
                        "Example project demonstrating eager and lazy-loaded routes.",
                        "https://example.com/performance/code-splitting-demo.zip"
                    ),
                    (
                        ResourceType.Link,
                        "Bundle Analysis Example",
                        "Example page for comparing bundle behavior before and after splitting.",
                        "https://example.com/performance/bundle-analysis"
                    )
                ],

                ["Vite Plugins & Environment Variables"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Environment Configuration",
                        """
                        Environment variables are useful when the same application needs different configuration values in development, testing, and production. Vite provides a straightforward mechanism for exposing selected environment values to the client application during the build process.
                        It is important to understand that a value used by browser code should never be considered a secret simply because it came from an environment file. Anything included in the final client bundle can potentially be inspected by users. Sensitive credentials belong in a server-side environment where the browser does not receive them.
                        Vite's plugin system extends the build tool without requiring the core project structure to become complicated. When evaluating a plugin, understand the problem it solves and document why your project needs it rather than adding dependencies without a clear purpose.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vite Environment Variables",
                        "Official documentation for environment variables and build modes.",
                        "https://vitejs.dev/guide/env-and-mode"
                    ),
                    (
                        ResourceType.Link,
                        "Vite Plugins",
                        "Reference for adding and configuring Vite plugins.",
                        "https://vitejs.dev/guide/using-plugins"
                    )
                ],

                ["Add Authentication-Aware Routing"] =
                [
                    (
                        ResourceType.Instruction,
                        "Assignment Instructions",
                        """
                        Extend the application with authentication-aware routing. Protected screens should only be rendered when the current user is authenticated. When an unauthenticated visitor attempts to access a protected location, redirect them to the login page and preserve enough information to return them to their intended destination after login.
                        Keep the route guard behavior centralized. The individual pages inside the protected area should not each need to know how authentication works. Your routing structure should communicate clearly which routes are public, which are protected, and where the authentication boundary is enforced.
                        Test both sides of the flow. Verify that an authenticated user can navigate normally, that an unauthenticated user cannot access protected content, and that direct navigation to a protected URL behaves correctly rather than relying only on navigation from links inside the application.
                        """,
                        null
                    ),
                    (
                        ResourceType.TextMaterial,
                        "Thinking About Route Guards",
                        """
                        Authentication-aware routing is fundamentally about controlling access to parts of the user interface based on application state. The important architectural question is where that decision should be made so that protected pages do not each implement their own version of the same rule.
                        A route guard provides a central boundary around protected content. It can inspect the current authentication state and either render the requested route or redirect the visitor somewhere appropriate. Keeping this logic centralized also makes the application's authorization behavior easier to test.
                        Be careful to distinguish authentication from authorization. Knowing that a user is logged in does not necessarily mean that the user is allowed to perform every action or visit every resource. For this assignment, focus primarily on the authentication boundary and leave more detailed role-based authorization for future work.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Router Tutorial",
                        "Primary routing reference for implementing the assignment.",
                        "https://reactrouter.com/en/main/start/tutorial"
                    ),
                    (
                        ResourceType.Link,
                        "Protected Route Example",
                        "Example protected-route flow for comparison.",
                        "https://example.com/auth/protected-routes"
                    ),
                    (
                        ResourceType.Link,
                        "Routing Assignment Rubric",
                        "Example PDF containing authentication and routing acceptance criteria.",
                        "https://example.com/assignments/auth-routing-rubric.pdf"
                    )
                ],

                ["Performance Profiling with React DevTools"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Profiling React Applications",
                        """
                        React DevTools can help you understand which components render, how often they render, and where unnecessary work may be occurring. The first step in performance work should always be measurement. Recording a profile before changing the code gives you evidence about the actual behavior of the application.
                        Look for components that render because of state or parent updates even though their visible output does not need to change. Then investigate why that render occurs before choosing an optimization. Sometimes the best fix is a different component boundary or a simpler state model rather than an optimization API.
                        After making a change, record another profile and compare the results. Performance work is most useful when you can explain the original problem, the change you made, and the measurable effect it had.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Developer Tools",
                        "Official React documentation for installing and using React DevTools.",
                        "https://react.dev/learn/react-developer-tools"
                    ),
                    (
                        ResourceType.Link,
                        "Profiling Checklist",
                        "Example checklist for investigating unnecessary renders.",
                        "https://example.com/performance/react-profiling.pdf"
                    )
                ],

                // ============================================================
                // MODULE 5
                // ============================================================

                ["Unit Testing Fundamentals with Vitest"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Writing Useful Unit Tests",
                        """
                        A useful unit test verifies behavior in a small, isolated piece of code. The test should make it clear what scenario is being exercised, what inputs are provided, and what result is expected. A common structure is arrange, act, and assert: prepare the inputs, execute the code under test, and verify the result.
                        Good tests are deterministic. They should not depend on the current time, network availability, random values, or another test having run first unless those dependencies are deliberately controlled. Tests should also avoid unnecessary knowledge of implementation details because implementation-focused tests become brittle when the code is refactored.
                        As you write your first Vitest tests, pay attention to naming. A test name should help a developer understand the expected behavior without opening the implementation. A well-written test suite can serve as executable documentation for the behavior of the code.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vitest Documentation",
                        "Primary reference for test files, assertions, mocking, and test execution.",
                        "https://vitest.dev/"
                    ),
                    (
                        ResourceType.Link,
                        "Unit Testing Fundamentals",
                        "Example PDF covering test structure and common assertion patterns.",
                        "https://example.com/testing/unit-testing-fundamentals.pdf"
                    )
                ],

                ["Component Testing with React Testing Library"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Testing from the User's Perspective",
                        """
                        React Testing Library is designed around the idea that tests should resemble the way users interact with the application. Users do not know that a component has a useState call or a particular internal object structure. They see buttons, headings, forms, messages, and other visible behavior.
                        This makes accessibility-oriented queries especially valuable. Querying by role or label often produces tests that are both more meaningful and more resilient to refactoring. If the internal implementation changes while the user-visible behavior remains the same, the test can continue to pass.
                        The intention is not to avoid all implementation knowledge in every possible test. It is to place most of your component-level confidence around the behavior that actually matters to someone using the application.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Testing Library",
                        "Official documentation and introduction to testing React components.",
                        "https://testing-library.com/docs/react-testing-library/intro/"
                    ),
                    (
                        ResourceType.Link,
                        "Queries Reference",
                        "Reference for choosing appropriate Testing Library queries.",
                        "https://testing-library.com/docs/queries/about/"
                    ),
                    (
                        ResourceType.Link,
                        "Component Tests ZIP",
                        "Example collection of React component tests.",
                        "https://example.com/testing/component-tests.zip"
                    )
                ],

                ["Writing Tests for the Shopping Cart Feature"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Testing the Shopping Cart",
                        """
                        A shopping cart gives you several layers of behavior to test. The reducer or state transition logic should be tested separately so that operations such as adding, removing, and changing quantities can be verified without involving the browser. The component layer can then focus on whether those operations are correctly exposed through the user interface.
                        Include edge cases in addition to the normal workflow. Think about an empty cart, minimum quantities, duplicate products, and the transition from one item remaining to no items remaining. These cases are often where business rules become visible.
                        Keep each test focused on one meaningful behavior. A concise test that clearly communicates its purpose is easier to maintain and more valuable than a large test that happens to execute many lines of code at once.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "React Testing Library",
                        "Reference for testing the cart UI from the user's perspective.",
                        "https://testing-library.com/docs/react-testing-library/intro/"
                    ),
                    (
                        ResourceType.Link,
                        "Vitest Assertions",
                        "Reference for assertions used in unit and component tests.",
                        "https://vitest.dev/api/expect.html"
                    )
                ],

                ["Achieve 80% Test Coverage on Core Components"] =
                [
                    (
                        ResourceType.Instruction,
                        "Assignment Instructions",
                        """
                        Increase the automated test coverage of the application's core components to at least 80 percent while keeping the tests focused on meaningful behavior. Begin by running the existing coverage report and identifying which components, branches, or functions contain the largest gaps.
                        Prioritize important application behavior rather than adding superficial assertions simply to increase the number. A good test should demonstrate something about the expected behavior of the application. Pay particular attention to conditional rendering, error states, user interactions, and other paths that are easy to overlook during normal development.
                        Submit the completed test suite together with the coverage report. Your submission should make it clear which parts of the application are considered core components and should demonstrate that the coverage target was reached without sacrificing test quality.
                        """,
                        null
                    ),
                    (
                        ResourceType.TextMaterial,
                        "Using Coverage Effectively",
                        """
                        Coverage reports measure which parts of the code executed while the test suite was running. They can show gaps in statements, branches, functions, and lines, but a high percentage does not automatically mean that the application is well tested.
                        Use the report as a diagnostic tool. A branch that is never exercised may represent an important error state, an empty result, or another scenario that users can encounter. On the other hand, a line that simply wires together two values may not deserve the same testing attention as a critical business rule.
                        The target for this assignment is 80 percent coverage, but the more important skill is learning how to interpret the report and decide where additional tests provide real confidence.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vitest Coverage",
                        "Official coverage configuration and reporting documentation.",
                        "https://vitest.dev/guide/coverage.html"
                    ),
                    (
                        ResourceType.Link,
                        "React Testing Library",
                        "Reference for behavior-focused component testing.",
                        "https://testing-library.com/docs/react-testing-library/intro/"
                    ),
                    (
                        ResourceType.Link,
                        "Example Coverage Report",
                        "Sample coverage report showing statements, branches, functions, and lines.",
                        "https://example.com/testing/coverage-report.pdf"
                    ),
                    (
                        ResourceType.Link,
                        "Testing Starter ZIP",
                        "Example test-suite structure for the assignment.",
                        "https://example.com/assignments/testing-starter.zip"
                    )
                ],

                ["Intro to End-to-End Testing with Playwright"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Understanding End-to-End Tests",
                        """
                        End-to-end tests validate a complete user journey through the application rather than one isolated function or component. A browser starts the application, performs realistic interactions, and verifies the resulting behavior. This allows you to catch integration problems that unit and component tests may not reveal.
                        End-to-end tests should focus on important workflows rather than attempting to reproduce every possible interaction. A login flow, creating a record, navigating between key pages, or completing a checkout-like process can provide high value because several parts of the application must work together for the journey to succeed.
                        Because these tests are more expensive to run, they should complement rather than replace faster tests. Use unit and component tests for detailed coverage and a smaller number of end-to-end tests to confirm that the major pieces work together in a real browser environment.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Playwright Documentation",
                        "Official Playwright documentation for browser-based end-to-end testing.",
                        "https://playwright.dev/"
                    ),
                    (
                        ResourceType.Link,
                        "End-to-End Example",
                        "Example browser workflow implemented as an end-to-end test.",
                        "https://example.com/testing/playwright-example"
                    )
                ],

                // ============================================================
                // MODULE 6
                // ============================================================

                ["Production Builds & Environment Configs in Vite"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Preparing a Production Build",
                        """
                        A production build is more than simply running a different command. The development environment provides tools that make local work convenient, while the production build creates the optimized assets that browsers will actually receive after deployment.
                        Inspect the generated build output and verify that the application still behaves correctly when served outside the development server. Pay attention to route handling, asset paths, environment-specific values, and any assumptions that only hold true during local development.
                        Remember that frontend environment variables are ultimately part of a client application when they are exposed to browser code. Never place credentials or other sensitive secrets into values that will be bundled into the production frontend.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vite Production Build",
                        "Official documentation covering Vite's production build process.",
                        "https://vitejs.dev/guide/build.html"
                    ),
                    (
                        ResourceType.Link,
                        "Environment Modes",
                        "Reference for environment-specific configuration in Vite.",
                        "https://vitejs.dev/guide/env-and-mode"
                    ),
                    (
                        ResourceType.Link,
                        "Production Checklist",
                        "Example PDF containing a pre-deployment verification checklist.",
                        "https://example.com/deployment/production-build-checklist.pdf"
                    )
                ],

                ["CI/CD Pipelines with GitHub Actions"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Continuous Integration and Delivery",
                        """
                        A CI/CD pipeline automates the repetitive steps that should happen whenever code changes. For a frontend application, a useful starting pipeline can install dependencies, run linting, execute automated tests, build the production application, and report failures back to the team.
                        The main benefit is not simply speed. Automation creates a consistent definition of what it means for code to be ready. Every change is checked using the same commands instead of depending on a developer remembering every step manually.
                        Good pipelines should also fail clearly. When a test or build fails, the developer should be able to see which stage failed and inspect enough information to diagnose the problem. Start with a small pipeline that is reliable and understandable before adding more complex deployment automation.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "GitHub Actions",
                        "Official documentation for building automated CI/CD workflows.",
                        "https://docs.github.com/actions"
                    ),
                    (
                        ResourceType.Link,
                        "Workflow Syntax",
                        "Reference for defining GitHub Actions jobs, steps, triggers, and configuration.",
                        "https://docs.github.com/actions/using-workflows/workflow-syntax-for-github-actions"
                    ),
                    (
                        ResourceType.Link,
                        "CI Starter ZIP",
                        "Example repository containing a minimal CI workflow.",
                        "https://example.com/ci/github-actions-starter.zip"
                    ),
                    (
                        ResourceType.Link,
                        "Pipeline Diagram",
                        "Example CI/CD pipeline diagram.",
                        "https://example.com/ci/pipeline-diagram.pdf"
                    )
                ],

                ["Deploying to Vercel & Netlify"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Understanding Frontend Deployment",
                        """
                        Deploying a frontend application means moving the build output from your local environment to a service that can deliver those files to real users. The deployment platform needs to know how to build the project, where the resulting files are located, and which configuration values should be available during the build.
                        After deployment, test the public application instead of assuming that a successful build means everything is correct. Navigate directly to important routes, refresh nested URLs, verify that images and other assets load correctly, and test the major workflows just as you would locally.
                        Comparing two hosting platforms is useful because it reveals which parts of deployment are platform-specific and which parts are requirements of the application itself. The goal is to become comfortable moving an application from development into a real production environment.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vercel Documentation",
                        "Official Vercel documentation for deploying frontend applications.",
                        "https://vercel.com/docs"
                    ),
                    (
                        ResourceType.Link,
                        "Netlify Documentation",
                        "Official Netlify documentation for deploying and configuring web applications.",
                        "https://docs.netlify.com/"
                    )
                ],

                ["Capstone Project: Build & Ship a Full Application"] =
                [
                    (
                        ResourceType.Instruction,
                        "Capstone Instructions",
                        """
                        Your capstone project brings together the major concepts from the course into one complete application. Build an application that demonstrates thoughtful React component architecture, TypeScript usage, client-side routing, state management, remote data fetching, automated testing, and production deployment.
                        Begin with a short planning phase before writing the full implementation. Define the main users and workflows, identify the data your application needs, sketch the route structure, and decide which state should remain local and which state needs to be shared. Think about testing and deployment early rather than leaving them until the final day.
                        The finished application should be usable from beginning to end. A reviewer should be able to start the application or visit the deployed version, follow the main workflow, and understand the major technical decisions you made. Include meaningful automated tests and deploy a working production build rather than submitting an unfinished development-only version.
                        """,
                        null
                    ),
                    (
                        ResourceType.TextMaterial,
                        "Planning a Production Application",
                        """
                        A larger application becomes much easier to build when its boundaries are decided before implementation grows. Start by identifying the smallest set of workflows that define success for your project. Those workflows can then guide the route structure, components, data model, and tests.
                        As you design the architecture, avoid treating every type of state as if it were the same. Local UI state, shared client state, and data retrieved from a server usually have different lifecycles and should be managed accordingly. Similarly, not every reusable component needs to become part of a large design system.
                        Keep the production environment in mind throughout the project. Configuration, error handling, loading states, testing, and deployment are part of the application rather than tasks that belong only to the final week. A small amount of planning early can prevent major restructuring later.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Vite Guide",
                        "Reference for project setup, configuration, and production builds.",
                        "https://vitejs.dev/guide/"
                    ),
                    (
                        ResourceType.Link,
                        "React Documentation",
                        "Primary React reference for component architecture and hooks.",
                        "https://react.dev/learn"
                    ),
                    (
                        ResourceType.Link,
                        "React Router",
                        "Reference for client-side routing and route composition.",
                        "https://reactrouter.com/"
                    ),
                    (
                        ResourceType.Link,
                        "TanStack Query",
                        "Reference for server-state management and data fetching.",
                        "https://tanstack.com/query/latest"
                    ),
                    (
                        ResourceType.Link,
                        "Capstone Requirements",
                        "Example capstone requirements and grading criteria.",
                        "https://example.com/capstone/project-requirements.pdf"
                    ),
                    (
                        ResourceType.Link,
                        "Capstone Starter ZIP",
                        "Optional example starter repository for students who want a baseline project.",
                        "https://example.com/capstone/starter-project.zip"
                    )
                ],

                ["Final Presentations & Demo Day"] =
                [
                    (
                        ResourceType.TextMaterial,
                        "Presenting Your Final Application",
                        """
                        A strong technical demonstration should make it easy for the audience to understand what your application does before you explain how it was built. Start with the main workflow and show the application solving the problem it was designed to address rather than beginning with a long tour of the codebase.
                        After the main demonstration, choose one or two technical decisions that are worth discussing. You might explain your component architecture, state-management strategy, handling of remote data, testing approach, or deployment setup. Focus on decisions that affected the project rather than trying to describe every library or file.
                        Finish by showing the deployed application and briefly discussing something you learned during the project. A good presentation is not about demonstrating every feature. It is about communicating the most important parts of the work clearly and confidently.
                        """,
                        null
                    ),
                    (
                        ResourceType.Link,
                        "Demo Day Checklist",
                        "Example PDF containing a checklist for preparing the technical presentation.",
                        "https://example.com/capstone/demo-day-checklist.pdf"
                    )
                ]
            };

        foreach (var module in course.Modules)
        {
            foreach (var activity in module.Activities)
            {
                if (!resourcesByActivity.TryGetValue(activity.Name, out var resources))
                {
                    throw new InvalidOperationException(
                        $"No activity resources configured for activity '{activity.Name}'.");
                }

                foreach (var resource in resources)
                {
                    activity.Resources.Add(
                        new ActivityResource
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            CreatedByUserId = teacherId,
                            ResourceType = resource.Type,
                            Name = resource.Name,
                            Description = resource.Description,
                            URL = resource.Url
                        });
                }
            }
        }
    }
}