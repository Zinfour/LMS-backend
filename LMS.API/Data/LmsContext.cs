using LMS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LMS.API.Data
{
    public class LmsContext(DbContextOptions<LmsContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        public DbSet<Course> Course { get; set; } = default!;
        public DbSet<Module> Module { get; set; } = default!;
        public DbSet<Activity> Activity { get; set; } = default!;
        public DbSet<Assignment> Assignment { get; set; } = default!;
        public DbSet<Submission> Submission { get; set; } = default!;

        public DbSet<ActivityResource> ActivityResource { get; set; } = default!;
        public DbSet<ActivityResource> CourseResource { get; set; } = default!;
        public DbSet<ActivityResource> ModuleResource { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<IdentityUserRole<string>>(
                    j => j.HasOne<ApplicationRole>().WithMany().HasForeignKey(ur => ur.RoleId),
                    j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey(ur => ur.UserId));
        }
    }
}
