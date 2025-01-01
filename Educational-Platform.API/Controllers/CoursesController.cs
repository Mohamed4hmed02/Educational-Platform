using Educational_Platform.Application.Abstractions.CourseInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
	[Route("api/v1/[controller]/")]
	[ApiController]
	public class CoursesController(
		ICourseCommandServices courseCommandServices,
		ICourseQueryServices courseQueryServices) : ControllerBase
	{
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async ValueTask AddCourse(CommandCourseModel course)
		{
			await courseCommandServices.CreateAsync(course);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut]
		public async ValueTask UpdateCourse(CommandCourseModel course)
		{
			await courseCommandServices.UpdateAsync(course);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{courseId}")]
		public async ValueTask DeleteCourse(int courseId)
		{
			await courseCommandServices.DeleteAsync(courseId);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{courseId}/Intro")]
		public async ValueTask SetIntroTopic(int topicId, int courseId)
		{
			await courseCommandServices.SetTopicIntroAsync(courseId, topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{courseId}/Image")]
		public async ValueTask SetImage(int courseId, IFormFile image)
		{
			await courseCommandServices.SetImageAsync(courseId, image);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{courseId}")]
		public async ValueTask SetVisibilty(int courseId, bool visible)
		{
			await courseCommandServices.SetVisibiltyAsync(courseId, visible);
		}

		[AllowAnonymous]
		[HttpGet]
		public async ValueTask<IActionResult> GetAllCourses(int page, int size, decimal? price, int comparison, string? title)
		{
			return Ok(await courseQueryServices.GetPageAsync(page, size, ModelTypeEnum.Query));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("CommandModels")]
		public async ValueTask<IActionResult> GetAllCoursesCommandModel(int page, int size, decimal? price, int comparison, string? title)
		{
			return Ok(await courseQueryServices.GetPageAsync(page, size, ModelTypeEnum.Command));
		}

		[AllowAnonymous]
		[HttpGet("{courseId}")]
		public async ValueTask<IActionResult> GetCourseInDetail(int courseId)
		{
			return Ok(await courseQueryServices.GetCourseInDtailAsync(courseId));
		}

		[AllowAnonymous]
		[HttpGet("Count")]
		public async ValueTask<IActionResult> GetCoursesCount()
		{
			return Ok(await courseQueryServices.GetCountAsync(ModelTypeEnum.Query));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("Count/CommandModels")]
		public async ValueTask<IActionResult> GetCommandCoursesCount()
		{
			return Ok(await courseQueryServices.GetCountAsync(ModelTypeEnum.Command));
		}
	}
}
