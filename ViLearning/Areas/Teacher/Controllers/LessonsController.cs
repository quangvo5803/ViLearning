using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class LessonsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BlobStorageService _blobStorageService;

        public LessonsController(IUnitOfWork unitOfWork, BlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }

        // GET: Teacher/Lessons
        public async Task<IActionResult> Index()
        {

            var applicationDBContext = _unitOfWork.Lesson.GetAll(includeProperties:"Course");
            /*var applicationDBContext = _context.Lessons.Include(l => l.Course);*/
            return View( applicationDBContext);
        }

        // GET: Teacher/Lessons/Details/5
        [HttpGet("Teacher/{CourseName}/{LessonName}/Detail")]
        public async Task<IActionResult> Details(string CourseName, string LessonName, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lesson = _unitOfWork.Lesson.Get(m => m.LessonId == id,includeProperties:"Course");
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Teacher/Lessons/Create
        [HttpGet("Teacher/{name}/Lessons/Create")]
        public IActionResult Create(string name)
        {
            
            ViewData["CourseId"] = new SelectList
                    (_unitOfWork.Course
                    .GetRange(c => c.CourseName.Equals(name)), "CourseId", "CourseName");
            var course = _unitOfWork.Course.Get(c => c.CourseName.Equals(name));
            ViewBag.Size = _unitOfWork.Lesson.GetRange(l => l.CourseId == course.CourseId).Count() + 1;
            return View();
        }

        [TempData]
        public string StatusMessage { get; set; }
        // POST: Teacher/Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Teacher/{name}/Lessons/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, [Bind("LessonId,LessonName,LessonNo,Content,Video,TotalQuestions,EasyQuestions,MediumQuestions,HardQuestions,TestDuration,CourseId")] Lesson lesson, IFormFile Video)
        {
            int courseId = _unitOfWork.Course.Get(c => c.CourseName.Equals(name)).CourseId;
            ModelState.Remove("Comments");
            if (ModelState.IsValid)
            {
                try
                {
                    string containerName = "lesson-video";
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Video.FileName);

                    // Check and delete old file from Azure Blob Storage
                    if (!string.IsNullOrEmpty(lesson.Video))
                    {
                        Uri oldUri = new Uri(lesson.Video);
                        string oldFileName = Path.GetFileName(oldUri.LocalPath);
                        await _blobStorageService.DeleteFileAsync(containerName, oldFileName);
                    }

                    // Upload new file to Azure Blob Storage
                    using (var stream = Video.OpenReadStream())
                    {
                        lesson.Video = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);
                    }

                    _unitOfWork.Lesson.Add(lesson);
                    _unitOfWork.Save();


                    return RedirectToAction("Details", "Courses", new { id = courseId});
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error uploading file: {ex.Message}";
                    Course course = _unitOfWork.Course.Get(c => c.CourseName.Equals(name));
                    _unitOfWork.Course.LoadCourse(course);
                    ViewBag.Size = _unitOfWork.Lesson.GetRange(l => l.CourseId == course.CourseId).Count() + 1;

                    ViewData["CourseId"] = new SelectList(_unitOfWork.Course.GetRange(c => c.CourseName.Equals(name)), "CourseId", "CourseName", lesson.CourseId);
                    return View(lesson);
                }
            }
            
            return View(lesson);
        }

        // GET: Teacher/Lessons/Edit/5
        [HttpGet("Teacher/{CourseName}/{LessonName}/Edit/{id}")]
        public async Task<IActionResult> Edit(string CourseName, string LessonName, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = _unitOfWork.Lesson.Get(c => c.LessonId == id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_unitOfWork.Course.GetAll(), "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }
        // POST: Teacher/Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Teacher/{CourseName}/{LessonName}/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string CourseName, string LessonName, int id, [Bind("LessonId,LessonName,LessonNo,Content,Video,TotalQuestions,EasyQuestions,MediumQuestions,HardQuestions,TestDuration,CourseId")] Lesson lesson, IFormFile Video)
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
                    try
                    {
                        string containerName = "lesson-video";
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Video.FileName);

                        // Check and delete old file from Azure Blob Storage
                        if (!string.IsNullOrEmpty(lesson.Video))
                        {
                            Uri oldUri = new Uri(lesson.Video);
                            string oldFileName = Path.GetFileName(oldUri.LocalPath);
                            await _blobStorageService.DeleteFileAsync(containerName, oldFileName);
                        }

                        // Upload new file to Azure Blob Storage
                        using (var stream = Video.OpenReadStream())
                        {
                            lesson.Video = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);
                        }

                        _unitOfWork.Lesson.Update(lesson);
                        _unitOfWork.Save();


                        return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
                    }
                    catch (Exception ex)
                    {
                        StatusMessage = $"Error uploading file: {ex.Message}";
                        return View(lesson);
                    }
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
                return RedirectToAction("Details","Courses",new {id = lesson.Course.CourseId});
            }
            
            return View(lesson);
        }

        // GET: Teacher/Lessons/Delete/5
        [HttpGet("Teacher/{CourseName}/{LessonName}/Delete/{id}")]
        public async Task<IActionResult> Delete(string CourseName, string LessonName,int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lesson = _unitOfWork.Lesson.Get(m => m.LessonId == id, includeProperties: "Course");
/*            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.LessonId == id);*/
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Teacher/Lessons/Delete/5
        [HttpPost("Teacher/{CourseName}/{LessonName}/Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string CourseName, string LessonName, int id)
        {
            var lesson = _unitOfWork.Lesson.Get(l => l.LessonId == id, includeProperties:"Course");
            if (lesson != null)
            {
                _unitOfWork.Lesson.Remove(lesson);
            }

            _unitOfWork.Save();
            
            return RedirectToAction("Details", "Courses", new { id = lesson.Course.CourseId });
        }

        private bool LessonExists(int id)
        {
            return (_unitOfWork.Lesson.Get(l => l.LessonId == id) != null);
        }
    }
}
