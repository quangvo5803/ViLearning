using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [Required]
        [Display(Name = "Tên khóa học")]
        public string CourseName { get; set; }
        [Display(Name = "Giá khóa học")]
        public double Price { get; set; }
        [Display(Name = "Mô tả khóa học")]
        public string Description { get; set; }
        [Display(Name = "Ảnh bìa khóa học")]
        [ValidateNever]
        public string CoverImgUrl { get; set; }


        //Foreign key
        public string SubjectId { get; set; }
        public string TeacherID { get; set; }


        //Relation
        [ForeignKey("SubjectId")]
        [ValidateNever]
        public Subject Subject { get; set; }      
        [ForeignKey("TeacherId")]
        [ValidateNever]     
        public ApplicationUser Teacher { get; set; }
        
        
        //Hiện tại còn thiếu Lesson,Certificate,Feedback
    }
}
