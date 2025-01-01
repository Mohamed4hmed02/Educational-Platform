using Educational_Platform.Application.Abstractions.UserCourseInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
	[Route("api/v1/[controller]/")]
	[ApiController]
	public class UsersCoursesController(
		IUserCourseCommandServices userCourseCommandServices,
		IUserCourseQueryServices courseQueryServices) : ControllerBase
	{
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async ValueTask AddCourseToUser(CommandUserCourseModel model)
		{
			await userCourseCommandServices.AddCourseToUserAsync(model);
		}

		[Authorize]
		[HttpGet]
		public async ValueTask<IActionResult> GetUserCourses(string userId)
		{
			return Ok(await courseQueryServices.GetUserCoursesAsync(userId));
		}

		[Authorize]
		[HttpGet("{courseId}")]
		public async ValueTask<IActionResult> GetUnitUserCanAccess(string userId, int courseId)
		{
			return Ok(await courseQueryServices.GetMaxUnitUserCanAccess(userId, courseId));
		}

		[Authorize]
		[HttpGet("{courseId}/Certificate")]
		public async ValueTask GetCertificate(string userId, int courseId)
		{
			await courseQueryServices.GetCertificateAsync(userId, courseId);
		}
	}
}
