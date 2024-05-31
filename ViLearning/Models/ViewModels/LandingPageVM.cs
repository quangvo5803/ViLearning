using Microsoft.AspNetCore.Mvc.Rendering;

namespace ViLearning.Models.ViewModels
{
    public class LandingPageVM
    {
        public List<ApplicationUser> UserList { get; set; }
        public List<ApplicationUser> TeacherList { get; set; }
        public List<Course> Courses { get; set; }
    }
}
