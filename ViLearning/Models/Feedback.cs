using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Feedback
    {
        [Key]
        public int FeedBackId { get; set; }
        [Range(1, 5)]
        public int FeedBackStar { get; set; }
        [Display(Name = "Đánh giá: ")]
        public string? FeedBackContent { get; set; }
        //Foreign key
        public string UserId { get; set; }
        public int CourseId { get; set; }

        public DateTime CreatedAt { get; set; }


        //Relation
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

    }
}