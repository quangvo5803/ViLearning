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
        public double Mark {  get; set; }

        //Foregin key
        public int LessonId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; } 
        //Relation
        [ForeignKey("LessonId")]
        [ValidateNever]
        public Lesson Lesson { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
		public Dictionary<int, string>? TestResult { get; set; }

		[NotMapped]
		public Dictionary<int, bool>? QuestionsIsCorrect { get; set; }

		[NotMapped]
		public ICollection<Question>? Questions { get; set; }
	}
}
