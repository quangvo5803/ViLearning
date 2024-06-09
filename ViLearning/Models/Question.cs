using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum QuestionType
    {
        MultipleChoice,
        Essay
    }
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string RightAnswer { get; set; }

        public QuestionType QuestionType { get; set; }
        public Difficulty Difficulty { get; set; }

        //Foreign key
        public int LessonId { get; set; }

        //Relation
        [ForeignKey("LessonId")]
        [ValidateNever]
        public Lesson Lesson { get; set; }
    }
}
