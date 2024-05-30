using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViLearning.Models;

namespace ViLearning.Data
{
    public class ApplicationDBContext :IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options){
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CourseCertificate> CourseCertificates { get; set; }
        public DbSet<StudentCertificate> StudentCertificates{ get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestDetail> TestDetails { get; set; }
        public DbSet<Content> Contents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Toán" },
                new Subject { Id = 2, Name = "Ngữ Văn" }
                );

            modelBuilder.Entity<StudentCertificate>()
            .HasKey(sc => new { sc.CourseCertificateId, sc.UserId });

            modelBuilder.Entity<StudentCertificate>()
                .HasOne(sc => sc.ApplicationUser)
                .WithMany()
                .HasForeignKey(sc => sc.UserId);

            modelBuilder.Entity<StudentCertificate>()
                .HasOne(sc => sc.CourseCertificate)
                .WithMany()
                .HasForeignKey(sc => sc.CourseCertificateId);

            modelBuilder.Entity<Feedback>()
            .HasKey(f => f.FeedBackId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.ApplicationUser)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Course)
                .WithMany()
                .HasForeignKey(f => f.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
        }
    }
}
