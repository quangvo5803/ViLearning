using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Drawing;
using System.Reflection.PortableExecutable;
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
        private readonly BlobStorageService _blobStorageService;

        public TestController(IUnitOfWork unitOfWork, BlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
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

			return View("DoTest", vm);
        }

        [HttpPost]
		public async Task<IActionResult> SubmitTest(DoTestVM vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lesson = _unitOfWork.Lesson.Get(l => l.LessonId == vm.Lesson.LessonId);
            TestDetail testDetail = new TestDetail()
			{
				StartTime = vm.TestDetail.StartTime,
				Duration = DateTime.Now - vm.TestDetail.StartTime,
				UserId = userId,
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
            // Update Learning Progress
            var learningProgress = _unitOfWork.LearningProgress.Get(q => q.UserId == testDetail.UserId && q.CourseId == testDetail.Lesson.CourseId, includeProperties:"User");
            int numOfLessonLearned = _unitOfWork.LearningProgress.NumOfLessonLearned(learningProgress); 
            double highestMarkOfLesson = await _unitOfWork.TestDetail.GetHighestMarkByLessonIdAsync(vm.Lesson.LessonId, userId);
            Course course = _unitOfWork.Course.Get(c => c.CourseId == lesson.CourseId, includeProperties: "ApplicationUser");
            learningProgress.Course = course;

            _unitOfWork.Course.LoadCourse(course);

            if (score >= vm.Lesson.TotalQuestions * 0.8 && !_unitOfWork.LearningProgress.HasLearnedLesson(learningProgress, lesson.LessonNo))
            {
                learningProgress.Progress += 100 / course.Lesson.Count;
                learningProgress.LearnedLessons += $"{lesson.LessonNo},";
            }

            if (score > highestMarkOfLesson )
            {
                learningProgress.OverallScore = (learningProgress.OverallScore* numOfLessonLearned + score) / numOfLessonLearned + 1;
            }

            if (learningProgress.Progress == 100) 
            {
                learningProgress.CompletionDate = DateTime.Now.Date;
                learningProgress.StudentCertificateUrl = await AssignCertificate(learningProgress);
            }
            
            _unitOfWork.LearningProgress.Update(learningProgress);
            // End Update Learning Progress

			_unitOfWork.TestDetail.Add(testDetail);
            _unitOfWork.Save();

			return View("TestResult", testDetail);
        }

        public async Task<string> AssignCertificate(LearningProgress learningProgress)
        {
            string templatePath = Path.Combine("wwwroot", "images", "ViLearning_Certificate_Template.pdf");

            // Check if File Exist
            if (!System.IO.File.Exists(templatePath))
            {
                return null;
            }
            PdfDocument document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
            PdfPage page = document.Pages[0];

            // Tạo đồ họa để vẽ
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Định nghĩa font và kích thước
            XFont font12 = new XFont("Arial", 12);
            XFont font24 = new XFont("Arial", 24);
            XFont font32 = new XFont("Arial", 32);

            // Định vị trí để thêm văn bản vào PDF (tọa độ x, y)
            XPoint datePosition = new XPoint(70, 75); // Vị trí của Complete Date
            XPoint namePosition = new XPoint(70, 128); // Vị trí của Student’s Name
            XPoint coursePosition = new XPoint(70, 192); // Vị trí của Course’s Name
            XPoint teacherPosition = new XPoint(290, 212); // Vị trí của Teacher’s Name

            // Thêm ngày hoàn thành
            gfx.DrawString(learningProgress.CompletionDate.ToString(), font12, XBrushes.Black, datePosition);

            // Thêm tên học viên
            gfx.DrawString(learningProgress.User.UserName, font32, XBrushes.Black, namePosition);

            // Thêm tên khóa học
            gfx.DrawString(learningProgress.Course.CourseName, font24, XBrushes.Black, coursePosition);

            // Thêm tên giáo viên
            gfx.DrawString(learningProgress.Course.ApplicationUser.UserName, font12, XBrushes.Black, teacherPosition);

            // Lưu tệp PDF mới lên Blob Storage
            // document.Save(outputPath);
            // Lưu tài liệu PDF vào MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0; // Đặt vị trí của stream về đầu

            // Tải file lên Blob Storage
            string containerName = "student-certificates-img";
            string fileName = $"{Guid.NewGuid()}.pdf";
            string fileUrl = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);

            return fileUrl;
        }
    }
}
