using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.Quizzes;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;

namespace Educational_Platform.Application.Services.QuizServices
{
	public class QuizzesQueryService(
		IUnitOfWork unitOfWork,
		IStorageService storageServices) : IQuizQueryService
	{
		public async ValueTask<CommandQuizModel> GetCommandQuizAsync(object unitId)
		{
			var unit = await unitOfWork.UnitsRepository
				.AsNoTracking()
				.ReadAsync(u => u.Id.Equals(unitId));

			return storageServices.GetQuiz(unit.QuizFileName);
		}

		public async ValueTask<QueryQuizModel?> GetQuizAsync(object unitId)
		{
			var unit = await unitOfWork.UnitsRepository
				.AsNoTracking()
				.ReadAsync(u => u.Id.Equals(unitId));

			return (QueryQuizModel)storageServices.GetQuiz(unit.QuizFileName);
		}
	}
}
