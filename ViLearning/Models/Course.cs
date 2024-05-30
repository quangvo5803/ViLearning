using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Tên khóa học: ")]
        public string CourseName { get; set; }
        [Display(Name = "Giá khóa học: ")]
        public double? Price { get; set; }
        [Display(Name = "Mô tả khóa học: ")]
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        [Display(Name = "Ảnh bìa khóa học: ")]
        public string CoverImgUrl { get; set; }


        //Foreign key
        public int SubjectId { get; set; }
        public string UserId { get; set; }


        //Relation
        [ForeignKey("SubjectId")]
        [ValidateNever]
        public Subject Subject { get; set; }      
        [ForeignKey("UserId")]
        [ValidateNever]     
        public ApplicationUser ApplicationUser { get; set; }
        
    }
}
