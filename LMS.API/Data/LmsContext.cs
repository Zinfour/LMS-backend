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
        public DbSet<Feedback> Feedback { get; set; } = default!;

        public DbSet<ActivityResource> ActivityResource { get; set; } = default!;
        public DbSet<CourseResource> CourseResource { get; set; } = default!;
        public DbSet<ModuleResource> ModuleResource { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Expose the UserRole table as the navigation properties Roles and Users on
            // ApplicationUser and ApplicationRole. This allows us to for example fetch
            // all users filtered by their roles without an explicit join.
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<IdentityUserRole<string>>(
                    j => j.HasOne<ApplicationRole>().WithMany().HasForeignKey(ur => ur.RoleId),
                    j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey(ur => ur.UserId));

            builder.Entity<ActivityResource>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ActivityResource>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<CourseResource>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseResource>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<ModuleResource>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModuleResource>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
