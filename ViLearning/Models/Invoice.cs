using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }
        //Foreign Key
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double Amount { get; set; }
        //Relation
        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
