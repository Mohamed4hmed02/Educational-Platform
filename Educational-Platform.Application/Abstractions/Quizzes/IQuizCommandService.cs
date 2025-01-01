using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Abstractions;

namespace Educational_Platform.Application.Abstractions.Quizzes
{
	public interface IQuizCommandService : IEditable<CommandQuizModel>, IDeleteable<CommandQuizModel>
	{
		ValueTask<QuizResultModel> ValidateAnswersAsync(QuizAnswerModel answerModel,int unitId);
	}
}
