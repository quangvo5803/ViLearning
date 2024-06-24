using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Authorization;
using ViLearning.Models.ViewModels;
using System;
namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = SD.Role_User_Teacher)]
    public class CoursesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BlobStorageService _blobStorageService;
        public CoursesController(IUnitOfWork unitOfWork, BlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }

        // GET: Teacher/Courses
        public IActionResult Index()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _unitOfWork.Course.GetAll().Where(c => c.UserId == currentUserId).ToList();
            var subject = _unitOfWork.Subject.GetAll().ToList();

            var viewModel = new CourseSubjectVM
            {
                Course = courses,
                Subject = subject
            };
            return View(viewModel);
        }

        // GET: Teacher/Courses/Details/5
        public IActionResult Details(int? id)   
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _unitOfWork.Course.Get(c => c.CourseId == id, includeProperties: "ApplicationUser,Subject");

            if (course == null)
            {
                return NotFound();
            }
            _unitOfWork.Course.LoadCourse(course);

            return View(course);
        }

        // GET: Teacher/Courses/Create
        public IActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(_unitOfWork.Subject.GetAll(), "Id", "Name");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userId;
            return View(new Course { UserId = userId });
        }

        // POST: Teacher/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseName,Price,Description,CoverImgUrl,SubjectId,Grade")] Course course, IFormFile coverImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            course.UserId = userId;

            if (coverImage != null && coverImage.Length > 0)
            {
                // Generate a unique file name
                string containerName = "course-cover-img";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);

                // Upload file to Azure Blob Storage
                using (var stream = coverImage.OpenReadStream())
                {
                    course.CoverImgUrl = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);
                }
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Course.Add(course);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            ViewBag.SubjectId = new SelectList(_unitOfWork.Subject.GetAll(), "Id", "Name", course.SubjectId);
            return View(course);
        }


        // GET: Teacher/Courses/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _unitOfWork.Course.Get(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.SubjectId = new SelectList(_unitOfWork.Subject.GetAll(), "Id", "Name", course.SubjectId);
            return View(course);
        }

        // POST: Teacher/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseName,Price,Description,CoverImgUrl,SubjectId,Grade")] Course course, IFormFile? coverImage)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            course.UserId = userId;

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        // Generate a unique file name
                        string containerName = "course-cover-img";
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);

                        // Delete the old file from Azure Blob Storage if exists
                        if (!string.IsNullOrEmpty(course.CoverImgUrl))
                        {
                            Uri oldUri = new Uri(course.CoverImgUrl);
                            string oldFileName = Path.GetFileName(oldUri.LocalPath);
                            await _blobStorageService.DeleteFileAsync(containerName, oldFileName);
                        }

                        // Upload the new file to Azure Blob Storage
                        using (var stream = coverImage.OpenReadStream())
                        {
                            course.CoverImgUrl = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);
                        }
                    }

                    _unitOfWork.Course.Update(course);
                    _unitOfWork.Save(); 
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Error updating course: {ex.Message}");
                   
                    ModelState.AddModelError(string.Empty, $"Error updating course: {ex.Message}");
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_unitOfWork.Subject.GetAll(), "Id", "Name", course.SubjectId);
            return View(course);
        }


        // GET: Teacher/Courses/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _unitOfWork.Course.Get(c => c.CourseId == id, includeProperties: "ApplicationUser,Subject");

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Teacher/Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _unitOfWork.Course.Delete(id);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: Teacher/Courses/Search
        public IActionResult Search(string searchString)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _unitOfWork.Course.GetAll().Where(c => c.UserId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.CourseName.Contains(searchString));
            }
            var viewModel = new CourseSubjectVM
            {
                Course = courses.ToList(),
                Subject = _unitOfWork.Subject.GetAll().ToList(),

            };

            return View("Index", viewModel);
        }

        // GET: Teacher/Courses/Filter
        public IActionResult Filter(string? subjectName, double? minPrice, double? maxPrice, string sortOrder, int? grade)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _unitOfWork.Course.GetAll().Where(c => c.UserId == userId);

            if (subjectName != null)
            {
                courses = courses.Where(c => c.Subject.Name == subjectName);
            }

            if (minPrice.HasValue)
            {
                courses = courses.Where(c => c.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                courses = courses.Where(c => c.Price <= maxPrice);
            }
            if (grade.HasValue && grade > 0)
            {
                courses = courses.Where(c => c.Grade == grade.Value);
            }

            switch (sortOrder)
            {
                case "asc":
                    courses = courses.OrderBy(c => c.Price);
                    break;
                case "desc":
                    courses = courses.OrderByDescending(c => c.Price);
                    break;
                default:
                    break;
            }

            var viewModel = new CourseSubjectVM
            {
                Course = courses,
                Subject = _unitOfWork.Subject.GetAll().ToList(),
                SelectedSubjectName = subjectName,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = sortOrder,
                Grade = grade
            };

      
            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult SubmitCourseContent(int courseId)
        {
            var course =  _unitOfWork.Course.Get(c => c.CourseId == courseId, includeProperties: "ApplicationUser,Lesson");

            if (course == null)
            {
                return NotFound("Khóa học không tồn tại.");
            }
           
            course.CourseStatus = CourseStatus.Pending;

            _unitOfWork.Course.Update(course);
            _unitOfWork.Save();


            TempData["SuccessMessage"] = "Đơn gửi đã được gửi thành công. Admin sẽ xem xét nội dung và phản hồi lại cho bạn sau.";

            return RedirectToAction("Details", new { id = courseId });
        }

    }
}
