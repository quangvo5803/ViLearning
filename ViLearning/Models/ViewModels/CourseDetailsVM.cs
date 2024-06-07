namespace ViLearning.Models.ViewModels
{
    public class CourseDetailsVM
    {
        public Course Course { get; set; }
        public IEnumerable<Lesson> Lessons { get; set; }
    }
}
