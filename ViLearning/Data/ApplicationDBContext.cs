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
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestDetail> TestDetails { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<LearningProgress> LearningProgresses { get; set; }
        public DbSet<WithdrawRequest> WithdrawRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configuration for LearningProgress 
            modelBuilder.Entity<LearningProgress>()
                .HasKey(lp => lp.LearningProgressId);
            // Configuration for Feedback
            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedBackId);


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

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Balance)
                .HasDefaultValue(0);
        }
    }
}
