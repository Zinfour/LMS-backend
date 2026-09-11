using LMS.API.Data;
using LMS.API.Models;
using LMS.API.Models.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MovieApi.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var context = services.GetRequiredService<LmsContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        if (!await context.Course.AnyAsync(cancellationToken))
        {
            await context.Course.AddAsync(GetPreconfiguredCourse(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await roleManager.RoleExistsAsync(Role.Teacher))
        {
            var result = await roleManager.CreateAsync(new ApplicationRole(Role.Teacher));
            if (!result.Succeeded)
                throw new Exception($"Failed to create role '{Role.Teacher}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await roleManager.RoleExistsAsync(Role.Student))
        {
            var result = await roleManager.CreateAsync(new ApplicationRole(Role.Student));
            if (!result.Succeeded)
                throw new Exception($"Failed to create role '{Role.Student}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var course = await context.Course.FirstAsync(c => c.Name == "Fullstack Developer", cancellationToken);

        var teacher = await userManager.FindByEmailAsync("teacher@gmail.com");
        if (teacher == null)
        {
            teacher = new ApplicationUser
            {
                UserName = "teacher@gmail.com",
                Email = "teacher@gmail.com",
                EmailConfirmed = true,
                FirstName = "Jane",
                LastName = "Teach",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CourseId = course.Id
            };

            var result = await userManager.CreateAsync(teacher, "Password123");
            if (!result.Succeeded)
                throw new Exception($"Failed to create teacher: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(teacher, Role.Teacher))
        {
            var result = await userManager.AddToRoleAsync(teacher, Role.Teacher);
            if (!result.Succeeded)
                throw new Exception($"Failed to assign teacher role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var student = await userManager.FindByEmailAsync("student@gmail.com");
        if (student == null)
        {
            student = new ApplicationUser
            {
                UserName = "student@gmail.com",
                Email = "student@gmail.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CourseId = course.Id
            };

            var result = await userManager.CreateAsync(student, "Password123");
            if (!result.Succeeded)
                throw new Exception($"Failed to create student: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(student, Role.Student))
        {
            var result = await userManager.AddToRoleAsync(student, Role.Student);
            if (!result.Succeeded)
                throw new Exception($"Failed to assign student role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await context.CourseResource.AnyAsync(cancellationToken))
        {
            AddResources(course, teacher.Id);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static void AddResources(Course course, string teacherId)
    {
        course.Resources.Add(new CourseResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Instruction,
            Name = "Course Guide",
            Description = "Overview of the course structure, modules, activities and learning objectives.",
            URL = null
        });
        course.Resources.Add(new CourseResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Reference,
            Name = "C# Documentation",
            Description = "Official Microsoft documentation for the C# programming language and .NET platform.",
            URL = "https://learn.microsoft.com/en-us/dotnet/csharp/"
        });

        var module1 = course.Modules.First(m => m.Name == "C# Fundamentals & OOP");
        module1.Resources.Add(new ModuleResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.TextMaterial,
            Name = "OOP Concepts Overview",
            Description = "A written overview of core object-oriented programming concepts: classes, objects, encapsulation and inheritance.",
            URL = null
        });
        module1.Resources.Add(new ModuleResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Summary,
            Name = "C# Quick Reference",
            Description = "Quick reference covering variables, control flow, methods and basic OOP syntax in C#.",
            URL = null
        });

        var githubDebug = module1.Activities.First(a => a.Name == "Getting Started: GitHub & Debugging");
        githubDebug.Resources.Add(new ActivityResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Instruction,
            Name = "Git & GitHub Guide",
            Description = "Getting started guide for Git and GitHub, covering repositories, commits and pull requests.",
            URL = "https://docs.github.com/en/get-started"
        });

        var garage = module1.Activities.First(a => a.Name == "Garage (Collections & Arrays)");
        garage.Resources.Add(new ActivityResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Link,
            Name = ".NET Collections Guide",
            Description = "Overview of collection types available in .NET, used in the Garage exercise.",
            URL = "https://learn.microsoft.com/en-us/dotnet/standard/collections/"
        });
    }

    private static Course GetPreconfiguredCourse()
    {
        var now = DateTime.UtcNow;

        return new Course
        {
            CreatedAt = now,
            UpdatedAt = now,
            Name = "Fullstack Developer",
            Description = "Fullstack development with C#, .NET, Entity Framework, REST APIs, HTML/CSS, TypeScript and React, concluding with a collaborative fullstack project.",
            StartDate = new DateOnly(2026, 4, 27),
            EndDate = new DateOnly(2026, 9, 16),
            ImageURL = "https://images.unsplash.com/photo-1461749280684-dccba630e2f6",
            Modules =
            [
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "C# Fundamentals & OOP",
                    Description = "Introduction to C# and core object-oriented programming, including version control, collections and the Garage exercise.",
                    StartDate = new DateOnly(2026, 4, 27),
                    EndDate = new DateOnly(2026, 5, 11),
                    ImageURL = "https://images.unsplash.com/photo-1555066931-4365d14bab8c",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Getting Started: GitHub & Debugging",
                            StartTime = new DateTime(2026, 4, 27, 9, 0, 0),
                            EndTime = new DateTime(2026, 4, 28, 16, 0, 0),
                            Description = "Course kickoff: setting up the development environment, an introduction to Git/GitHub, and debugging in Visual Studio."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Personalregister",
                            StartTime = new DateTime(2026, 4, 28, 13, 0, 0),
                            EndTime = new DateTime(2026, 4, 29, 9, 0, 0),
                            Description = "Build a simple console-based personnel register to practice fundamental C# syntax, control flow and input handling.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Personalregister",
                                Description = "Implement a personnel register application in C# and be ready to walk through your solution.",
                                Deadline = new DateTime(2026, 4, 29, 9, 0, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Object-Oriented Programming",
                            StartTime = new DateTime(2026, 5, 5, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 6, 16, 0, 0),
                            Description = "Core object-oriented programming concepts in C#: classes, objects, encapsulation and inheritance."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Garage (Collections & Arrays)",
                            StartTime = new DateTime(2026, 5, 7, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 11, 16, 0, 0),
                            Description = "Build a Garage application storing vehicles in arrays and collections, introducing collection types in C#."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Advanced C# & Clean Code",
                    Description = "Progressing from C# fundamentals into reusable, testable and maintainable code with generics, LINQ, unit testing and clean code principles.",
                    StartDate = new DateOnly(2026, 5, 12),
                    EndDate = new DateOnly(2026, 5, 27),
                    ImageURL = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Generics & Garage<T>",
                            StartTime = new DateTime(2026, 5, 12, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 15, 16, 0, 0),
                            Description = "Rebuild the Garage exercise using generics and interfaces to create a reusable, type-safe collection.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Garage<T>",
                                Description = "Refactor the Garage application to use generics, and be ready to explain your design choices.",
                                Deadline = new DateTime(2026, 5, 15, 16, 0, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "LINQ",
                            StartTime = new DateTime(2026, 5, 18, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 18, 16, 0, 0),
                            Description = "Practical exercises querying and transforming collections using LINQ."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Garage 2.0 & Unit Testing",
                            StartTime = new DateTime(2026, 5, 19, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 20, 16, 0, 0),
                            Description = "Extend Garage with unit tests, covering exception handling with try/catch/finally and test-driven practices.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Garage 2.0 with Unit Tests",
                                Description = "Add unit tests to the Garage<T> solution and handle invalid input using exceptions.",
                                Deadline = new DateTime(2026, 5, 20, 16, 0, 0),
                                Submissions = []
                            }
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "SOLID, Clean Code & Async/Await",
                            StartTime = new DateTime(2026, 5, 26, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 27, 16, 0, 0),
                            Description = "Principles of clean, maintainable code using SOLID, and an introduction to asynchronous programming with async/await."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Entity Framework & APIs",
                    Description = "Connecting application logic to persistent data with Entity Framework and exposing functionality through REST APIs.",
                    StartDate = new DateOnly(2026, 5, 28),
                    EndDate = new DateOnly(2026, 6, 26),
                    ImageURL = "https://images.unsplash.com/photo-1558494949-ef010cbdcc31",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Entity Framework Fundamentals",
                            StartTime = new DateTime(2026, 5, 28, 9, 0, 0),
                            EndTime = new DateTime(2026, 5, 29, 16, 0, 0),
                            Description = "Introduction to Entity Framework Core for data persistence and database access in .NET applications."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "API & EF Integration",
                            StartTime = new DateTime(2026, 6, 1, 9, 0, 0),
                            EndTime = new DateTime(2026, 6, 12, 16, 0, 0),
                            Description = "Building a web API backed by Entity Framework Core, connecting endpoints to a persistent database."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "REST API Development",
                            StartTime = new DateTime(2026, 6, 15, 9, 0, 0),
                            EndTime = new DateTime(2026, 6, 26, 16, 0, 0),
                            Description = "Designing and implementing RESTful API endpoints following REST conventions."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Frontend Fundamentals",
                    Description = "Transitioning from backend development into browser-based frontend development with HTML, CSS and TypeScript.",
                    StartDate = new DateOnly(2026, 6, 29),
                    EndDate = new DateOnly(2026, 7, 8),
                    ImageURL = "https://images.unsplash.com/photo-1523437113738-bbd3cc89fb19",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "HTML & CSS",
                            StartTime = new DateTime(2026, 6, 29, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 2, 16, 0, 0),
                            Description = "Building semantic, styled web pages with HTML and CSS, including responsive layout basics."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "TypeScript Fundamentals",
                            StartTime = new DateTime(2026, 7, 3, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 8, 16, 0, 0),
                            Description = "Introduction to TypeScript's type system and how it improves JavaScript development."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "React Development",
                    Description = "Building component-based frontend applications with React and TypeScript.",
                    StartDate = new DateOnly(2026, 7, 9),
                    EndDate = new DateOnly(2026, 8, 21),
                    ImageURL = "https://images.unsplash.com/photo-1592609931095-54a2168ae893",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "React Fundamentals",
                            StartTime = new DateTime(2026, 7, 9, 9, 0, 0),
                            EndTime = new DateTime(2026, 7, 17, 16, 0, 0),
                            Description = "Introduction to React, JSX, and building component-based user interfaces."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Components & State",
                            StartTime = new DateTime(2026, 8, 3, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 14, 16, 0, 0),
                            Description = "Managing component state and props, and structuring reusable React components with TypeScript."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "React Application Development",
                            StartTime = new DateTime(2026, 8, 17, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 21, 16, 0, 0),
                            Description = "Building a complete React application that integrates components, state and API calls."
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Fullstack Project",
                    Description = "Applying accumulated backend and frontend knowledge in a final collaborative fullstack project.",
                    StartDate = new DateOnly(2026, 8, 24),
                    EndDate = new DateOnly(2026, 9, 16),
                    ImageURL = "https://images.unsplash.com/photo-1611224923853-80b023f02d71",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Project Planning & Scrum",
                            StartTime = new DateTime(2026, 8, 24, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 24, 16, 0, 0),
                            Description = "Sprint planning using Scrum, setting up a GitHub Project board and Trello board for the team."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "GitHub Projects & Issues",
                            StartTime = new DateTime(2026, 8, 25, 9, 0, 0),
                            EndTime = new DateTime(2026, 8, 28, 16, 0, 0),
                            Description = "Managing tasks and collaboration using GitHub Projects and Issues throughout the sprint."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "LMS Fullstack Development",
                            StartTime = new DateTime(2026, 8, 31, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 15, 16, 0, 0),
                            Description = "Collaborative development of the Course Portal (LMS) fullstack application, applying backend and frontend skills from the course."
                        }
                    ]
                }
            ]
        };
    }
}