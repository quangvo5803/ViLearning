using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class CourseCertificate
    {
        [Key]
        public int CertificateId {  get; set; }
        [Required]
        public string CertificateName { get; set; }

        //Foreign key
        public int CourseId { get; set; }
     
        //Relaiton
        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

    }
}
