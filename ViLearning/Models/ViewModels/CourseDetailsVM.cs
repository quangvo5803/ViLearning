namespace ViLearning.Models.ViewModels
{
    public class CourseDetailsVM
    {
        public Course Course { get; set; }
        public IEnumerable<Lesson> Lessons { get; set; }

        public Invoice Invoice { get; set; }
        public Feedback Feedback { get; set; }
        public FeedbacksViewModel? Feedbacks { get; set; }

        public LearningProgress? LearningProgress { get; set; }
    }
}
