using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class WithdrawRequest
    {
        [Key]
        public int WithdrawRequestID { get; set; }

        [Required]
        public string UserId { get; set; }

        public double RequestMoney {  get; set; }
        public DateTime RequestDay { get; set; }
        public bool Status { get; set; }
        public DateTime? CompleteDay {  get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
