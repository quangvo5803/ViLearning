using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViLearning.Models;

namespace ViLearning.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CourseCertificate> CourseCertificates { get; set; }
        public DbSet<StudentCertificate> StudentCertificates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestDetail> TestDetails { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           




            // Configuration for StudentCertificate
            modelBuilder.Entity<StudentCertificate>()
                .HasKey(sc => new { sc.CourseCertificateId, sc.UserId });

            modelBuilder.Entity<StudentCertificate>()
                .HasOne(sc => sc.ApplicationUser)
                .WithMany()
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes from ApplicationUser

            modelBuilder.Entity<StudentCertificate>()
                .HasOne(sc => sc.CourseCertificate)
                .WithMany()
                .HasForeignKey(sc => sc.CourseCertificateId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading deletes from CourseCertificate

            // Configuration for Feedback
            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedBackId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.ApplicationUser)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes from ApplicationUser

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Course)
                .WithMany()
                .HasForeignKey(f => f.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading deletes from Course

            // Configuration for Comment
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.CommetId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Lesson)
                .WithMany(l => l.Comments) // Assuming Lesson has a collection of Comments
                .HasForeignKey(c => c.LessonId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading deletes from Lesson

            // Additional configurations to avoid cycles
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Lesson)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Prevent cascading deletes from Course to Lessons

            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Comments)
                .WithOne(c => c.Lesson)
                .HasForeignKey(c => c.LessonId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading deletes from Lesson to Comments

			modelBuilder.Entity<Lesson>()
			    .HasOne(l => l.Course)
			    .WithMany(c => c.Lesson)
			    .HasForeignKey(l => l.CourseId)
			    .OnDelete(DeleteBehavior.Cascade);
		}
    }
}
