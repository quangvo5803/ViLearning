using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class TestDetail
    {
        [Key]
        public int TestDetailId { get; set; }
        public int NumberQuestion { get; set; }
        [Range(0,10)]
        public double Mark {  get; set; }

        //Foregin key
        public int LessonId { get; set; }
        public string UserId { get; set; }

        //Relation
        [ForeignKey("LessonId")]
        [ValidateNever]
        public Lesson Lesson { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
