using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViLearning.Data;
using ViLearning.Models;

namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class LessonsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public LessonsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Teacher/Lessons
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Lessons.Include(l => l.Course);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Teacher/Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Teacher/Lessons/Create
        [HttpGet("Teacher/Courses/{name}/Create")]
        public IActionResult Create(string name)
        {
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.CourseName.Equals(name)).ToList(), "CourseId", "CourseName");
            return View();
        }

        // POST: Teacher/Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Teacher/Courses/{name}/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, [Bind("LessonId,LessonName,NumberOfQuestion,LessonNo,Content,CourseId")] Lesson lesson)
        {
            ModelState.Remove("Comments");
            if (ModelState.IsValid)
            {
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.CourseName.Equals(name)).ToList(), "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Teacher/Lessons/Edit/5
        [HttpGet("Teacher/Courses/{name}/Lessons/{id}")]
        public async Task<IActionResult> Edit(string name, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Teacher/Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Teacher/Courses/{name}/Lessons/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LessonId,LessonName,NumberOfQuestion,LessonNo,Content,CourseId")] Lesson lesson)
        {
            if (id != lesson.LessonId)
            {
                return NotFound();
            }
            ModelState.Remove("Comments");
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.LessonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View(lesson);
        }

        // GET: Teacher/Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Teacher/Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.LessonId == id);
        }
    }
}
