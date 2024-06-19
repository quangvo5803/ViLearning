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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewUploadQuestionsFile(int courseId)
        {
            ViewData["courseId"] = courseId;
            return View("CourseQuestionBank");
        }

        [HttpPost]
        public async Task<IActionResult> UploadQuestions(int courseId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var questions = new List<Question>();
            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                Question question = null;
                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (question != null)
                        {
                            questions.Add(question);
                            question = null;
                        }
                        continue;
                    }

                    if (question == null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length < 3)
                        {
                            // Handle invalid line format
                            return BadRequest("Invalid file format.");
                        }
                        // Check if lessonNo in the file is valid.
                        var lesson = _unitOfWork.Lesson.Get(l => l.CourseId == courseId && l.LessonNo == int.Parse(parts[0]));
                        if (lesson == null)
                        {
                            // Skip this question if the lesson is not found
                            while (!stream.EndOfStream && !string.IsNullOrWhiteSpace(line))
                            {
                                line = await stream.ReadLineAsync();
                            }
                            continue;
                        }

                        question = new Question
                        {
                            LessonId = _unitOfWork.Lesson.Get(l => l.CourseId == courseId && l.LessonNo == int.Parse(parts[0])).LessonId,
                            QuestionType = Enum.Parse<QuestionType>(parts[1]),
                            Difficulty = Enum.Parse<Difficulty>(parts[2])
                        };
                        line = await stream.ReadLineAsync();
                        question.QuestionName = line;
                    }
                    else
                    {
                        if (line.StartsWith("A."))
                        {
                            question.OptionA = line.Substring(2).Trim();
                        }
                        else if (line.StartsWith("B."))
                        {
                            question.OptionB = line.Substring(2).Trim();
                        }
                        else if (line.StartsWith("C."))
                        {
                            question.OptionC = line.Substring(2).Trim();
                        }
                        else if (line.StartsWith("D."))
                        {
                            question.OptionD = line.Substring(2).Trim();
                        }
                        else if (line.StartsWith("Correct:"))
                        {
                            question.RightAnswer = line.Substring(8).Trim();
                        }
                    }
                }

                if (question != null)
                {
                    questions.Add(question);
                }
            }

            _unitOfWork.Question.AddRange(questions);
            _unitOfWork.Save();

            return RedirectToAction("ViewUploadQuestionsFile", courseId);
        }

        public async Task<IActionResult> QuestionManage(int lessonId)
        {
            QuestionManageVM vm = new QuestionManageVM();
            vm.Questions = await _unitOfWork.Question.GetQuestionByLessonId(lessonId);
            vm.Lesson = _unitOfWork.Lesson.Get(l => l.LessonId == lessonId, includeProperties: "Course");
           
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(QuestionManageVM vm)
        {
            if (vm.Question.QuestionType == QuestionType.Essay) 
                vm.Question.RightAnswer = "";
            _unitOfWork.Question.Add(vm.Question);
            _unitOfWork.Save();
            int lessonId = vm.Question.LessonId;

            return RedirectToAction("QuestionManage", new { lessonId = lessonId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(QuestionManageVM vm)
        {
            if (vm.Question.QuestionType == QuestionType.Essay) 
                vm.Question.RightAnswer = "";
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

        [HttpGet]
        public async Task<IActionResult> Search(QuestionManageVM vm)
        {
            if (vm.SearchString == null) vm.SearchString = "";
            vm.Questions = _unitOfWork.Question.GetRange(q => q.LessonId == vm.Question.LessonId && q.QuestionName.Contains(vm.SearchString))
                .OrderBy(x => x.Difficulty).ThenBy(x => x.LessonId).ToList();
            vm.Lesson = _unitOfWork.Lesson.Get(l => l.LessonId == vm.Question.LessonId, includeProperties: "Course");

            return View("QuestionManage",vm);
        }
    }
}
