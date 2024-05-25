using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateID { get; set; }
        [Required]
        public string CertificateName { get; set; }
        public string UserID { get; set; }
        [ForeignKey("Id")]
        public ApplicationUser applicationUser { get; set; }
        public string CoverImgUrl { get; set; }
    }
}
