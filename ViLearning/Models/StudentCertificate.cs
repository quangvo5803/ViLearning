using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class StudentCertificate
    {
        //Foreign key
       
        public int CourseCertificateId { get; set; }
        
        public string UserId { get; set; }

        //Relation
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("CourseCertificateId")]
        [ValidateNever]
        public CourseCertificate CourseCertificate { get; set; }
    }
}
