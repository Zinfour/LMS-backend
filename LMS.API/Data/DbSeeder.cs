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

    // How many activities (in module order, out of Module 1/2/3/4) each student has completed so far.
    // Modules 5 and 6 haven't started yet, so nobody has activity there.
    private static readonly int[][] StudentProgress =
    [
        [6, 7, 5, 2], // student@gmail.com  - on track, currently working through the routing module
        [6, 7, 6, 3], // student1@gmail.com - ahead of schedule
        [6, 7, 5, 2], // student2@gmail.com - on track
        [6, 6, 3, 0], // student3@gmail.com - fell behind during module 2
        [6, 7, 1, 0], // student4@gmail.com - barely started module 3
        [4, 0, 0, 0], // student5@gmail.com - struggling since module 1
        [6, 7, 6, 6], // student6@gmail.com - finished everything released so far
        [6, 7, 4, 1], // student7@gmail.com - average pace
        [6, 7, 6, 3], // student8@gmail.com - caught back up on module 3
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

        // Already seeded, nothing left to do.
        if (await context.Course.AnyAsync(cancellationToken))
        {
            return;
        }

        var course = BuildCourse();
        context.Course.Add(course);
        await context.SaveChangesAsync(cancellationToken);

        var teacher = await CreateUserAsync(userManager, "teacher@gmail.com", "Jordan", "Blake", Role.Teacher, course.Id, imageUrl: null);

        var students = new List<ApplicationUser>();
        for (var i = 0; i < StudentNames.Length; i++)
        {
            var email = i == 0 ? "student@gmail.com" : $"student{i}@gmail.com";
            var (firstName, lastName) = StudentNames[i];
            students.Add(await CreateUserAsync(userManager, email, firstName, lastName, Role.Student, course.Id, imageUrl: null));
        }

        AddResources(course, teacher.Id);
        await context.SaveChangesAsync(cancellationToken);

        SeedStudentProgress(context, course, students);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string firstName,
        string lastName,
        string role,
        int courseId,
        string? imageUrl)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
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
                            Text = $"Submission from {student.FirstName} {student.LastName} for \"{activity.Assignment.Title}\".",
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

        AddAssignmentResource(modules[0], "Build a Typed Utility Library", teacherId,
            "https://www.typescriptlang.org/docs/handbook/2/generics.html");
        AddAssignmentResource(modules[1], "Build a Product Catalog UI", teacherId,
            "https://react.dev/learn/passing-props-to-a-component");
        AddAssignmentResource(modules[2], "Build a Paginated Product List with React Query", teacherId,
            "https://tanstack.com/query/latest/docs/framework/react/guides/paginated-queries");
        AddAssignmentResource(modules[3], "Add Authentication-Aware Routing", teacherId,
            "https://reactrouter.com/en/main/start/tutorial");
        AddAssignmentResource(modules[4], "Achieve 80% Test Coverage on Core Components", teacherId,
            "https://vitest.dev/guide/coverage.html");
        AddAssignmentResource(modules[5], "Capstone Project: Build & Ship a Full Application", teacherId,
            "https://vitejs.dev/guide/");
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

    private static void AddAssignmentResource(Module module, string activityName, string teacherId, string url)
    {
        var activity = module.Activities.First(a => a.Name == activityName);
        activity.Resources.Add(new ActivityResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Instruction,
            Name = "Assignment Instructions",
            Description = $"Reference material to help with the \"{activityName}\" assignment.",
            URL = url
        });
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
            ImageURL = "https://picsum.photos/seed/react-ts-vite-course/1200/600",
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
}