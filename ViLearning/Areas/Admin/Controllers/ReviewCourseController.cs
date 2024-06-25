using Microsoft.AspNetCore.Mvc;
using ViLearning.Data;
using ViLearning.Models;
using System.Linq;
using ViLearning.Services.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using ViLearning.Utility;
using System;


[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ReviewCourseController : Controller
{


    private readonly IUnitOfWork _unitOfWork;
    public ReviewCourseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(int? id)
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

    [HttpGet]
    public IActionResult LessonDetails(int id)
    {
        var lesson = _unitOfWork.Lesson.Get(m => m.LessonId == id, includeProperties: "Course");
        if (lesson == null)
        {
            return NotFound("Bài học không tồn tại.");
        }

        return View("LessonDetails", lesson); // Chỉ rõ view trong thư mục Views/ReviewCourse
    }

    [HttpGet]
    public async Task<IActionResult> Details(string CourseName, string LessonName, int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var lesson = _unitOfWork.Lesson.Get(m => m.LessonId == id, includeProperties: "Course");
        if (lesson == null)
        {
            return NotFound();
        }

        return RedirectToAction("LessonDetails", new { id = lesson.LessonId });
    }


   
}