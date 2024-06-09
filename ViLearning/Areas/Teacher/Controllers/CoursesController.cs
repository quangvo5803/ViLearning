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
            return View(courses);
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
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseName,Price,Description,CoverImgUrl,SubjectId,Grade")] Course course, IFormFile coverImage)
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
            var course = _unitOfWork.Course.Get(c => c.CourseId == id);
            if (course != null)
            {
                _unitOfWork.Course.Remove(course);
                _unitOfWork.Save();
            }
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

            ViewBag.Subjects = _unitOfWork.Subject.GetAll();
            return View("Index", courses.ToList());
        }

        // GET: Teacher/Courses/Filter
        public IActionResult Filter(int? subjectId, double? minPrice, double? maxPrice, string sortOrder)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _unitOfWork.Course.GetAll().Where(c => c.UserId == userId);

            if (subjectId.HasValue && subjectId > 0)
            {
                courses = courses.Where(c => c.SubjectId == subjectId);
            }

            if (minPrice.HasValue)
            {
                courses = courses.Where(c => c.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                courses = courses.Where(c => c.Price <= maxPrice);
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

            ViewBag.SelectedSubjectId = subjectId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Subjects = _unitOfWork.Subject.GetAll();

            return View("Index", courses.ToList());
        }
    }
}
