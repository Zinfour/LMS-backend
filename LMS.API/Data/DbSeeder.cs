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

        var course = await context.Course.FirstAsync(c => c.Name == "Machine Learning Fundamentals", cancellationToken);

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
            Description = "Overview of the course structure, expectations, grading, and learning objectives.",
            URL = "https://example.com/ml/course-guide"
        });
        course.Resources.Add(new CourseResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Reference,
            Name = "Python Documentation",
            Description = "Official Python documentation for students who need to refresh their Python skills.",
            URL = "https://docs.python.org/3/"
        });

        var introduction = course.Modules.First(m => m.Name == "Introduction to Machine Learning");
        introduction.Resources.Add(new ModuleResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.TextMaterial,
            Name = "What Is Machine Learning?",
            Description = "A beginner-friendly introduction to machine learning and its applications.",
            URL = "https://example.com/ml/introduction"
        });
        introduction.Resources.Add(new ModuleResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Summary,
            Name = "ML Concepts Cheat Sheet",
            Description = "Quick reference covering datasets, features, labels, models, and training.",
            URL = "https://example.com/ml/cheatsheet"
        });

        var seminar = introduction.Activities.First(a => a.Name == "What Is Machine Learning?");
        seminar.Resources.Add(new ActivityResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Instruction,
            Name = "Seminar Instructions",
            Description = "Review the pre-reading material before attending the seminar.",
            URL = "https://example.com/ml/seminar-1"
        });

        var workflow = introduction.Activities.First(a => a.Name == "Machine Learning Workflow");
        workflow.Resources.Add(new ActivityResource
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = teacherId,
            ResourceType = ResourceType.Link,
            Name = "Interactive ML Workflow",
            Description = "Interactive visualization of a typical machine learning workflow.",
            URL = "https://example.com/ml/workflow"
        });
    }

    private static Course GetPreconfiguredCourse()
    {
        var now = DateTime.UtcNow;

        return new Course
        {
            CreatedAt = now,
            UpdatedAt = now,
            Name = "Machine Learning Fundamentals",
            Description = "An introduction to machine learning covering supervised learning, unsupervised learning, model evaluation, and practical applications.",
            StartDate = new DateOnly(2026, 9, 1),
            EndDate = new DateOnly(2026, 12, 18),
            ImageURL = "https://images.unsplash.com/photo-1555255707-c07966088b7b",
            Modules =
            [
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Introduction to Machine Learning",
                    Description = "Core concepts, terminology, and the machine learning workflow.",
                    StartDate = new DateOnly(2026, 9, 1),
                    EndDate = new DateOnly(2026, 9, 25),
                    ImageURL = "https://images.unsplash.com/photo-1518770660439-4636190af475",
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "What Is Machine Learning?",
                            StartTime = new DateTime(2026, 9, 8, 10, 0, 0),
                            EndTime = new DateTime(2026, 9, 8, 12, 0, 0),
                            Description = "Introduction to machine learning terminology, workflows, and real-world use cases."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Machine Learning Workflow",
                            StartTime = new DateTime(2026, 9, 10, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 17, 23, 59, 0),
                            Description = "Self-paced lesson covering data collection, preprocessing, training, evaluation, and deployment."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Build Your First Classifier",
                            StartTime = new DateTime(2026, 9, 15, 9, 0, 0),
                            EndTime = new DateTime(2026, 9, 25, 23, 59, 0),
                            Description = "Build and evaluate a simple classification model using a provided dataset.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "First Classification Model",
                                Description = "Train a simple classifier, evaluate its performance, and explain your results.",
                                Deadline = new DateTime(2026, 9, 25, 23, 59, 0),
                                Submissions = []
                            }
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Supervised Learning",
                    Description = "Regression, classification, model training, and evaluation.",
                    StartDate = new DateOnly(2026, 9, 28),
                    EndDate = new DateOnly(2026, 10, 30),
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Seminar,
                            Name = "Regression and Classification",
                            StartTime = new DateTime(2026, 10, 1, 10, 0, 0),
                            EndTime = new DateTime(2026, 10, 1, 12, 0, 0),
                            Description = "Explore the differences between regression and classification problems."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Model Evaluation Workshop",
                            StartTime = new DateTime(2026, 10, 8, 13, 0, 0),
                            EndTime = new DateTime(2026, 10, 8, 15, 0, 0),
                            Description = "Hands-on practice with accuracy, precision, recall, F1 score, and cross-validation."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "House Price Prediction",
                            StartTime = new DateTime(2026, 10, 12, 9, 0, 0),
                            EndTime = new DateTime(2026, 10, 30, 23, 59, 0),
                            Description = "Build a regression model to predict house prices.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "House Price Prediction",
                                Description = "Train and evaluate a regression model using a house price dataset.",
                                Deadline = new DateTime(2026, 10, 30, 23, 59, 0),
                                Submissions = []
                            }
                        }
                    ]
                },
                new Module
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Name = "Neural Networks",
                    Description = "An introduction to neural networks, backpropagation, and deep learning.",
                    StartDate = new DateOnly(2026, 11, 2),
                    EndDate = new DateOnly(2026, 12, 18),
                    Activities =
                    [
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.ELearning,
                            Name = "Introduction to Neural Networks",
                            StartTime = new DateTime(2026, 11, 2, 9, 0, 0),
                            EndTime = new DateTime(2026, 11, 13, 23, 59, 0),
                            Description = "Learn the fundamentals of neural networks and how they learn from data."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Practice,
                            Name = "Build a Neural Network",
                            StartTime = new DateTime(2026, 11, 16, 13, 0, 0),
                            EndTime = new DateTime(2026, 11, 16, 15, 0, 0),
                            Description = "Implement a small neural network and experiment with hyperparameters."
                        },
                        new Activity
                        {
                            CreatedAt = now,
                            UpdatedAt = now,
                            Type = ActivityType.Assignment,
                            Name = "Image Classification Project",
                            StartTime = new DateTime(2026, 11, 23, 9, 0, 0),
                            EndTime = new DateTime(2026, 12, 18, 23, 59, 0),
                            Description = "Train a neural network to classify images.",
                            Assignment = new Assignment
                            {
                                CreatedAt = now,
                                UpdatedAt = now,
                                Title = "Image Classification",
                                Description = "Create, train, and evaluate a neural network for image classification.",
                                Deadline = new DateTime(2026, 12, 18, 23, 59, 0),
                                Submissions = []
                            }
                        }
                    ]
                }
            ]
        };
    }
}