using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Comment
    {
        [Key]
        public int CommetId { get; set; }
        public string? CommentContent { get; set; }
        public DateTime DateComment { get; set; }

        

        //Foregin key
        public int? LessonId { get; set; }
        public string? UserId { get; set; }
        public int? ParentCommentId { get; set; }

        //Relation
        [ForeignKey("LessonId")]
        [ValidateNever]
        public Lesson Lesson { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ParentCommentId")]
        [ValidateNever]
        public Comment ParentComment { get; set; }
        // Navigation property
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
