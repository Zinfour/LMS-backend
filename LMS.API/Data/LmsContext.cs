using LMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Data
{
    public class LmsContext(DbContextOptions<LmsContext> options) : DbContext(options)
    {
        public DbSet<User> User { get; set; } = default!;
        public DbSet<Course> Course { get; set; } = default!;
        public DbSet<Module> Module { get; set; } = default!;
        public DbSet<Activity> Activity { get; set; } = default!;
        public DbSet<Assignment> Assignment { get; set; } = default!;
        public DbSet<Submission> Submission { get; set; } = default!;

        public DbSet<ActivityResource> ActivityResource { get; set; } = default!;
        public DbSet<ActivityResource> CourseResource { get; set; } = default!;
        public DbSet<ActivityResource> ModuleResource { get; set; } = default!;
    }
}
