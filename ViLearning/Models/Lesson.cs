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

        
        [Required]
        public int LessonNo { get; set; }
        [Required]
        public string Content { get; set; }

        public string? Video { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng câu hỏi phải có dạng số nguyên dương.")]
        public int TotalQuestions { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng câu hỏi nhận biết phải có dạng số nguyên dương.")]
        public int EasyQuestions { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng câu hỏi thông hiểu phải có dạng số nguyên dương.")]
        public int MediumQuestions { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng câu hỏi vận dụng phải có dạng số nguyên dương.")]
        public int HardQuestions { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Thời gian bài kiểm tra phải có dạng số nguyên dương.")]
        public int TestDuration { get; set; }  // Duration in minutes

        //Foreign key
        public int CourseId { get; set; }

        //Relation
        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        //Navigation property
        public virtual ICollection<Comment>? Comments { get;}
        public virtual ICollection<Question>? Questions { get;}

        public virtual ICollection<TestDetail>? Tests { get; set; }

    }
}
