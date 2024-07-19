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
        public string? StudentCertificateUrl { get; set; }
        [ValidateNever]
        public Course Course { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; }

    }
}
