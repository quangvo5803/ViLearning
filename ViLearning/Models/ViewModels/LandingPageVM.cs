using Microsoft.AspNetCore.Mvc.Rendering;

namespace ViLearning.Models.ViewModels
{
    public class LandingPageVM
    {
        public IEnumerable<ApplicationUser> UserList { get; set; }
        public IEnumerable<ApplicationUser> TeacherList { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
