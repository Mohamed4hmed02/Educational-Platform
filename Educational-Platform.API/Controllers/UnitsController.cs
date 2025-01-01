using Educational_Platform.Application.Abstractions.Quizzes;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class UnitsController(
		IUnitCommandServices unitCommandServices,
		IUnitQueryServices unitQueryServices,
		IQuizCommandService questionsCommand,
		IQuizQueryService questionsQuery) : ControllerBase
	{
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async ValueTask AddUnit(CommandUnitModel unit)
		{
			await unitCommandServices.CreateAsync(unit);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("Quizzes")]
		public async ValueTask AddQuiz(CommandQuizModel models)
		{
			await questionsCommand.CreateAsync(models);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut]
		public async ValueTask UpdateUnit(CommandUnitModel unit)
		{
			await unitCommandServices.UpdateAsync(unit);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{unitId}")]
		public async ValueTask DeleteUnit(int unitId)
		{
			await unitCommandServices.DeleteAsync(unitId);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{unitId}/Quizzes")]
		public async ValueTask DeleteQuiz(int unitId)
		{
			await questionsCommand.DeleteAsync(unitId);
		}

		[Authorize(Policy = "UnitAccess")]
		[HttpPatch("{unitId}/Quizzes")]
		public async ValueTask<IActionResult> ValidateAnswers(QuizAnswerModel model, int unitId)
		{
			return Ok(await questionsCommand.ValidateAnswersAsync(model, unitId));
		}

		[Authorize]
		[HttpGet("{courseId}")]
		public async ValueTask<IActionResult> GetUnits(int courseId)
		{
			return Ok(await unitQueryServices.GetUnitsAsync(courseId, ModelTypeEnum.Query));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("{courseId}/CommandModels")]
		public async ValueTask<IActionResult> GetUnitsCommandModel(int courseId)
		{
			return Ok(await unitQueryServices.GetUnitsAsync(courseId, ModelTypeEnum.Command));
		}

		[Authorize(Policy = "UnitAccess")]
		[HttpGet("{unitId}/Quizzes")]
		public async ValueTask<IActionResult> GetQuiz(int unitId)
		{
			return Ok(await questionsQuery.GetQuizAsync(unitId));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("{unitId}/Quizzes/CommandModels")]
		public async ValueTask<IActionResult> GetCommandQuiz(int unitId)
		{
			return Ok(await questionsQuery.GetCommandQuizAsync(unitId));
		}
	}
}
