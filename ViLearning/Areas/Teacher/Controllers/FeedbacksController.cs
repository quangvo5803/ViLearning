using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList;
using ViLearning.Data;
using ViLearning.Models;

namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDBContext _context;

        public FeedbacksController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {


            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var teacherCourses = _context.Courses
                .Where(c => c.ApplicationUser.Id == currentUserId)
                .Select(c => c.CourseId)
                .ToList();

           
            var feedbacks = _context.Feedbacks
                .Include(f => f.ApplicationUser)
                .Include(f => f.Course)
                .Where(f => teacherCourses.Contains(f.CourseId))
                .OrderByDescending(f => f.CreatedDate)
                ;

            return View(feedbacks);
        }
    }
}
