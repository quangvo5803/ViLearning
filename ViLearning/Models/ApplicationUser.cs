using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? FullName { get; set; }

        
        public int? Age { get; set; }
        [DisplayName("Ngày sinh: ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        [DisplayName("Giới  tính: ")]
        public Gender? Gender { get; set; }
        public bool? TeacherCertificate { get; set; }
        public string? TeacherCertificateImgUrl { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public string RoleID { get; set; }

        //Navigation property
        public virtual ICollection<Course>? Courses { get; set; } 
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<Comment>? Comments {  get; set; } 
        public virtual ICollection<TestDetail>? TestDetail { get; set; }
        public virtual ICollection<StudentCertificate>? StudentCertificates { get; set; }
    }

    
    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
