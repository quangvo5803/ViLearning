using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class LearningProgress
    {
        [Key]
        public int LearningProgressId { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public double Progress { get; set; }
        public double OverallScore { get; set; }
        public DateTime EnrollDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? StudentCertificateUrl { get; set; }
        public string LearnedLessons { get; set; }

        public bool IsEmailCert { get; set; }

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }

    }
}
