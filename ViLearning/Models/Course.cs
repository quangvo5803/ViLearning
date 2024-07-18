using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public enum CourseStatus
    {
        Default,
        Rejected,
        Pending, 
        Published
    }
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Tên khóa học: ")]
        public string CourseName { get; set; }
        [Display(Name = "Giá khóa học: ")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá khóa học phải lớn hơn 0.")]
        public double? Price { get; set; }
        [Display(Name = "Mô tả khóa học: ")]
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        [Display(Name = "Ảnh bìa khóa học: ")]
        public string? CoverImgUrl { get; set; }

        public CourseStatus CourseStatus { get; set; } = CourseStatus.Default;


        //Foreign key
        public int SubjectId { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Grade là bắt buộc.")]
        [Range(1, 12, ErrorMessage = "Grade phải nằm trong khoảng từ 1 đến 12.")]
        public int Grade {  get; set; }
        /*public CourseStatus status {  get; set; }*/
        //Relation
        [ForeignKey("SubjectId")]
        [ValidateNever]
        public Subject Subject { get; set; }      
        [ForeignKey("UserId")]
        [ValidateNever]     
        public ApplicationUser ApplicationUser { get; set; }

        //Navigation property
        public virtual ICollection<Lesson>? Lesson { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        
        public virtual ICollection<LearningProgress>? LearningProgresses { get; set; }
    }
}
