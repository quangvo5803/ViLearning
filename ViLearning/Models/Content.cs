using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Content
    {
        [Key]
        public int ContentId { get; set; }
        public string Heading { get; set; }
        [Column(TypeName = "ntext")]
        [DisplayName("Nội dung khóa học")]
        public string MainContent { get; set; }

        //Foreign key
        public int LessonId { get; set; }

        //Relation
        [ForeignKey("LessonId")]
        [ValidateNever]
        public Lesson Lesson { get; set; }
    }
}
