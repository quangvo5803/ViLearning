using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using System.Security.Claims;
using ViLearning.Models;
using ViLearning.Models.ViewModels;


namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = SD.Role_User_Teacher)]
    public class QuestionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> QuestionManage(int lessonId)
        {
            QuestionManageVM vm = new QuestionManageVM();
            vm.Questions = await _unitOfWork.Question.GetQuestionByLessonId(lessonId);
            vm.Lesson = _unitOfWork.Lesson.Get(l => l.LessonId == lessonId);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(QuestionManageVM vm)
        {
            _unitOfWork.Question.Add(vm.Question);
            _unitOfWork.Save();
            int lessonId = vm.Question.LessonId;

            return RedirectToAction("QuestionManage", new { lessonId = lessonId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(QuestionManageVM vm)
        {
            _unitOfWork.Question.Update(vm.Question);
            _unitOfWork.Save();
            int lessonId = vm.Question.LessonId;

            return RedirectToAction("QuestionManage", new { lessonId = lessonId });
        }

        public async Task<IActionResult> Delete(int questionId)
        {
            Question question = _unitOfWork.Question.Get(q => q.QuestionId == questionId);
            Lesson lesson = _unitOfWork.Lesson.Get(l => l.LessonId == question.LessonId);
            _unitOfWork.Question.Remove(question);
            _unitOfWork.Save();
            

            return RedirectToAction("QuestionManage", new { lessonId = lesson.LessonId });
        }

        
    }
}
