using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace ViLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = SD.Role_User_Student)]
    public class TestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TestController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewTestInfo(int lessonId)
        {
            Lesson lesson = _unitOfWork.Lesson.Get(l => l.LessonId == lessonId, includeProperties:"Course");
            return View("ViewTestInfo", lesson);
        }

        public async Task<IActionResult> DoTest(int lessonId, DateTime startTime)
        {
            DoTestVM vm = new DoTestVM();
            vm.TestDetail = new TestDetail()
            {
                StartTime = startTime
            };
            vm.Lesson = _unitOfWork.Lesson.Get(l => l.LessonId == lessonId, includeProperties: "Course");
            // Get questions for the test
            int numOfHardQuestions = vm.Lesson.HardQuestions;
            int numOfMediumQuestions = vm.Lesson.MediumQuestions;
            int numOfEasyQuestions = vm.Lesson.EasyQuestions;
            List<Question> ques = await _unitOfWork.Question.GetQuestionByLessonId(lessonId);
            Random random = new Random();

            List<Question> hardQuestions = ques.Where(q => q.Difficulty == Difficulty.Hard && q.QuestionType == QuestionType.MultipleChoice)
                                                .OrderBy(q => random.Next())
                                                .Take(numOfHardQuestions)
                                                .ToList();

            List<Question> mediumQuestions = ques.Where(q => q.Difficulty == Difficulty.Medium && q.QuestionType == QuestionType.MultipleChoice)
                                                  .OrderBy(q => random.Next())
                                                  .Take(numOfMediumQuestions)
                                                  .ToList();

            List<Question> easyQuestions = ques.Where(q => q.Difficulty == Difficulty.Easy && q.QuestionType == QuestionType.MultipleChoice)
                                                .OrderBy(q => random.Next())
                                                .Take(numOfEasyQuestions)
                                                .ToList();

            vm.Questions = easyQuestions.Concat(mediumQuestions)
                                        .Concat(hardQuestions)
                                        .ToList();
            vm.TestDetail.TestResult = vm.Questions.ToDictionary(q => q.QuestionId, q => string.Empty); // Initialize dictionary with empty strings

			return View("DoTest", vm);
        }

        [HttpPost]
		public async Task<IActionResult> SubmitTest(DoTestVM vm)
        {
			TestDetail testDetail = new TestDetail()
			{
				StartTime = vm.TestDetail.StartTime,
				Duration = DateTime.Now - vm.TestDetail.StartTime,
				UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
				LessonId = vm.Lesson.LessonId,
                Questions = new List<Question>(),
                QuestionsIsCorrect = new Dictionary<int, bool>(),
                TestResult = new Dictionary<int, string>()
			};
            // Get score of testDetail and store question result (True/False)
			int score = 0;
            if (vm.TestDetail.TestResult != null)
            {
                foreach (KeyValuePair<int, string> kvp in vm.TestDetail.TestResult)
                {
                    Question question = _unitOfWork.Question.Get(q => q.QuestionId == kvp.Key);
                    testDetail.Questions.Add(question);
                    if (kvp.Value == question.RightAnswer)
                    {
                        score++;
                        testDetail.QuestionsIsCorrect.Add(kvp.Key, true);
                    }
                    else
                    {
                        testDetail.QuestionsIsCorrect.Add(kvp.Key, false);
                    }
                }
            }
            testDetail.Mark = score; 
            // End Get Score
            testDetail.Lesson = _unitOfWork.Lesson.Get(l => l.LessonId == vm.Lesson.LessonId, includeProperties: "Course");
            testDetail.TestResult = vm.TestDetail.TestResult;
			_unitOfWork.TestDetail.Add(testDetail);
            _unitOfWork.Save();

			return View("TestResult", testDetail);
        }

        public async Task<IActionResult> TestHistory(int lessonId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<TestDetail> testDetails = _unitOfWork.TestDetail.GetRange(t => t.LessonId == lessonId && t.UserId == userId,
                includeProperties: "ApplicationUser,Lesson")
                .OrderByDescending(t => t.StartTime).ToList();
            return View("TestHistory", testDetails);
        }

        public async Task<IActionResult> TestRanking(int lessonId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<TestDetail> testDetails = _unitOfWork.TestDetail.GetRange(t => t.LessonId == lessonId,
                includeProperties: "ApplicationUser,Lesson")
                .OrderByDescending(t => t.Mark).ThenBy(t => t.Duration).ThenBy(t => t.StartTime).ToList();
            return View("TestRanking", testDetails);
        }
    }
}
