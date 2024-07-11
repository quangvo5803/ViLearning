using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ViLearning.Data;
using ViLearning.Models;
using System.Threading.Tasks;
using ViLearning.Services.Repository.IRepository;
using System.Security.Claims;

namespace ViLearning.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FeedbacksController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		public FeedbacksController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			
		}

		[HttpPost]
		public async Task<IActionResult> CreateFeedback([FromBody] Feedback feedback)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { success = false, error = "Invalid data." });
			}


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            feedback.UserId = userId;

			 _unitOfWork.Feedback.Add(feedback);
			 _unitOfWork.Save();

			var response = new
			{
				success = true,
				feedback = new
				{
					feedback.FeedBackId,
					feedback.FeedBackStar,
					feedback.FeedBackContent,
					feedback.CourseId,
					feedback.CreatedDate,
					ApplicationUser = new
					{
						user.UserName
					}
				}
			};

			return Ok(response);
		}


	}
}
