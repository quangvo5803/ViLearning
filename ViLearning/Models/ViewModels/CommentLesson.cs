using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ViLearning.Models.ViewModels
{
    public class CommentLesson
    {
        public Course course { get; set; }
        public Lesson? Lesson { get; set; }
        public Comment Comment { get; set; }
    }
}
