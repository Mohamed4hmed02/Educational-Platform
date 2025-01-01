using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.Quizzes
{
	public interface IQuizQueryService
	{
		ValueTask<QueryQuizModel?> GetQuizAsync(object unitId);
		ValueTask<CommandQuizModel> GetCommandQuizAsync(object unitId);
	}
}
