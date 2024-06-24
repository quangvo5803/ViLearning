namespace ViLearning.Models.ViewModels
{
    public class CourseQuestionBankVM
    {
        public Course Course { get; set; }
        public List<Question> Questions { get; set; }

        public Question Question { get; set; }

        public Lesson Lesson { get; set; }

        public String SearchString { get; set; }
    }
}
