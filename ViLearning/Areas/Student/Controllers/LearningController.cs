using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace ViLearning.Areas.Student.Controllers
{
	[Area("Student")]
	[Authorize(Roles = SD.Role_User_Student)]
	public class LearningController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly List<string> _bannedWords = new List<string> { "fake", "999", "888" };

        public LearningController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int courseId, int lessonNo)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _unitOfWork.Course.Get( c => c.CourseId == courseId, includeProperties: "ApplicationUser");
            List<Lesson>? lessons = await _unitOfWork.Lesson.GetLessonByCourseId(courseId);
            var lesson = lessons?.Where(l => l.LessonNo == lessonNo).FirstOrDefault();
            var lessonId = _unitOfWork.Lesson.Get(l => l.CourseId == courseId && l.LessonNo == lessonNo).LessonId;
            var tests = _unitOfWork.TestDetail.GetRange(t => t.LessonId == lessonId);
            ViewData["userId"] = userId;
            foreach (Lesson l in lessons)
            {
                _unitOfWork.Lesson.LoadTest(l);
            }

            var id = GetLessonId(courseId, lessonNo);

            var lesson1 = _unitOfWork.Lesson.Get(m => m.LessonId == id, includeProperties: "Course,Comments,Comments.ApplicationUser,Comments.Replies");
            CommentsController commentsController = new CommentsController(_unitOfWork);
            var viewModel = new CommentLesson
            {
                Lesson = lesson1,
                Comment = new Comment { LessonId = lesson1.LessonId }
            };


            LearningMaterial lm = new LearningMaterial()
            {
                Course = course,
                Lesson = lesson,
                ListLesson = lessons,
                TestHistory = TestHistory(lessonId, userId),
                TestRanking = TestRanking(lessonId),
                commentLesson = viewModel
            };
            return View(lm);
        }
        public List<TestDetail> TestHistory(int lessonId, string userId)
        {
            List<TestDetail> testHistory = _unitOfWork.TestDetail.GetRange(t => t.LessonId == lessonId && t.UserId == userId,
                includeProperties: "ApplicationUser,Lesson")
                .OrderByDescending(t => t.StartTime).ToList();
            return testHistory;
        }

        public List<TestDetail> TestRanking(int lessonId)
        {
            List<TestDetail> testRanking = _unitOfWork.TestDetail.GetRange(t => t.LessonId == lessonId,
                includeProperties: "ApplicationUser,Lesson")
                .OrderByDescending(t => t.Mark).ThenBy(t => t.Duration).ThenBy(t => t.StartTime).ToList();
            return testRanking;
        }
        public int GetLessonId(int courseId, int lessonNo)
        {
            var lessonId = _unitOfWork.Lesson.Get(l => l.CourseId == courseId && l.LessonNo == lessonNo).LessonId;
            return lessonId;
        }

        [HttpPost]
        public IActionResult AddComment([FromBody] CommentLesson? model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (ContainsBannedWords(model.Comment.CommentContent))
            {
                return Json(new { success = false, error = "Bình luận chứa từ ngữ không phù hợp." });
            }

            model.Comment.UserId = userId;
            model.Comment.DateComment = DateTime.Now;

            _unitOfWork.Comment.Add(model.Comment);
            _unitOfWork.Save();

            var comment = _unitOfWork.Comment.Get(c => c.CommetId == model.Comment.CommetId, includeProperties: "ApplicationUser");
            return Json(new { success = true, comment });
        }

        [HttpPost]
        public IActionResult EditComment([FromBody] Comment model)
        {
            if (model == null)
            {
                return Json(new { success = false, error = "Model is null" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var commentFromDb = _unitOfWork.Comment.Get(c => c.CommetId == model.CommetId);
            if (commentFromDb == null)
            {
                return Json(new { success = false, error = "No comment found with the given CommetId" });
            }

            if (commentFromDb.UserId != userId)
            {
                return Json(new { success = false, error = "Unauthorized action" });
            }

            commentFromDb.CommentContent = model.CommentContent;
            _unitOfWork.Comment.Update(commentFromDb);
            _unitOfWork.Save();

            var updatedComment = _unitOfWork.Comment.Get(c => c.CommetId == model.CommetId, includeProperties: "ApplicationUser");

            return Json(new { success = true, comment = updatedComment });
        }



        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            var comment = _unitOfWork.Comment.Get(c => c.CommetId == id, includeProperties: "Replies"); // Include child replies
            if (comment == null)
            {
                return Json(new { success = false, error = "Comment not found" });
            }

            try
            {
                // Delete all replies recursively
                DeleteReplies(comment.Replies);

                // Remove the main comment
                _unitOfWork.Comment.Remove(comment);
                _unitOfWork.Save();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                return Json(new { success = false, error = "Failed to delete comment" });
            }
        }

        private bool ContainsBannedWords(string commentContent)
        {
            foreach (var word in _bannedWords)
            {
                if (commentContent.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteReplies(IEnumerable<Comment> replies)
        {
            foreach (var reply in replies)
            {
                var childReplies = _unitOfWork.Comment.GetAll().Where(c => c.ParentCommentId == reply.CommetId);
                DeleteReplies(childReplies);
                _unitOfWork.Comment.Remove(reply);
            }
        }
    }
}
