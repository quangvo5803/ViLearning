using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        [Required]
        public string LessonName { get; set; }
        public bool statusBoolean { get; set; }

        public int NumberOfQuestion { get; set; }

        //Foreign key
        public int CourseId { get; set; }

        //Relation
        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        //Navigation property
        public virtual ICollection<Comment> Comments { get;}
        public virtual ICollection<Question>? Questions { get;}
    }
}
