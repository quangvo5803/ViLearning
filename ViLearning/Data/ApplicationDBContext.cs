using Microsoft.EntityFrameworkCore;
using ViLearning.Models;

namespace ViLearning.Data
{
    public class ApplicationDBContext :DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options){
        }

        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Toán" },
                new Subject { Id = 2, Name = "Ngữ Văn" }
                );
        }
    }
}
