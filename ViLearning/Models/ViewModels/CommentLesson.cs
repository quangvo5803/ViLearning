using System.ComponentModel.DataAnnotations;

namespace ViLearning.Models.ViewModels
{
    public class CommentLesson
    {
        public Lesson? Lesson { get; set; }
        public Comment Comment { get; set; }
    }
}
