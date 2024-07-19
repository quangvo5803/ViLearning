namespace ViLearning.Models.ViewModels
{
    public class LearningMaterial
    {
        public Course Course { get; set; }
        
        public Lesson Lesson { get; set; }

        public List<TestDetail> TestRanking { get; set; }

        public List<TestDetail> TestHistory { get; set; }

        public List<Lesson> ListLesson { get; set; }
        public CommentLesson commentLesson { get; set; }

    }
}
