using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList;
using ViLearning.Data;
using ViLearning.Models;

namespace ViLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDBContext _context;

        public FeedbacksController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1);

            var feedbacks = _context.Feedbacks
                .Include(f => f.ApplicationUser)
                .Include(f => f.Course)
                .OrderByDescending(f => f.CreatedDate)
                .ToPagedList(pageNumber, pageSize);

            return View(feedbacks);
        }

    }
}
