using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.Quizzes;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Educational_Platform.Application.Services.QuizServices
{
	public class QuizzesCommandService(
		IUnitOfWork unitOfWork,
		IStorageService storageServices,
		ILogger<QuizzesCommandService> logger) : IQuizCommandService
	{
		public async ValueTask CreateAsync(CommandQuizModel entity)
		{
			if (entity.Questions == null || !entity.Questions.Any())
				throw new InvalidDataException("Can't add embty quizes")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

			var unit = await unitOfWork.UnitsRepository.ReadAsync(entity.UnitId);

			// assign ids to each question in quiz
			int id = 1;
			foreach (var question in entity.Questions)
			{
				question.Id = id++;
			}

			byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(entity));
			using var stream = new MemoryStream(data);
			unit.QuizFileName = await storageServices.SaveFileAsync(stream, $"U{entity.UnitId}-Quiz.json", FileTypeEnum.Json);
			await unitOfWork.SaveChangesAsync();
			logger.LogInformation("A quiz is added to for unit:{@unit}", unit);
		}

		public async ValueTask UpdateAsync(CommandQuizModel entity)
		{
			await CreateAsync(entity);
		}

		public async ValueTask DeleteAsync(object Id)
		{
			var unit = await unitOfWork.UnitsRepository.DeleteAsync(Id);
			await storageServices.DeleteFileAsync(unit.QuizFileName, FileTypeEnum.Json);
			await unitOfWork.SaveChangesAsync();
			logger.LogInformation("A quiz was deleted from unit:{@unit}", unit);
		}

		public async ValueTask<QuizResultModel> ValidateAnswersAsync(QuizAnswerModel answerModel, int unitId)
		{
			var user = await unitOfWork.UsersRepository
				.AsNoTracking()
				.ReadAsync(u => u.FullId == answerModel.UserId);

			var unit = await unitOfWork.UnitsRepository
				.AsNoTracking()
				.ReadAsync(u => u.Id == unitId);

			var quiz = storageServices.GetQuiz(unit.QuizFileName);

			// ensure same number of questions
			if (answerModel == null
				|| quiz == null
				|| quiz.Questions == null
				|| answerModel.Answers == null
				|| answerModel.Answers.Count() != quiz.Questions.Count())
				throw new InvalidDataException("Wrong number of answers are given")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

			// ensure the same id is matching
			answerModel.Answers = answerModel.Answers.OrderBy(a => a.QuestionId);
			quiz.Questions = quiz.Questions.OrderBy(q => q.Id);

			// check answers
			var result = new QuizResultModel
			{
				IsSuccess = true,
				UnitId = quiz.UnitId
			};
			for (int i = 0; i < quiz.Questions.Count(); i++)
			{
				var quizAnswer = quiz.Questions.ElementAt(i);
				var givenAnswer = answerModel.Answers.ElementAt(i);

				if (quizAnswer.Id != givenAnswer.QuestionId)
					throw new InvalidDataException($"Question with id {quizAnswer.Id} has no answer")
						.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

				// check answers correction
				var wrongAnswer = new Answer
				{
					QuestionId = givenAnswer.QuestionId
				};
				foreach (var chosen in givenAnswer.ChosenAnswers)
				{
					if (!quizAnswer.CorrectAnswers.Contains(chosen))
						wrongAnswer.ChosenAnswers.Add(chosen);
				}
				if (wrongAnswer.ChosenAnswers.Count > 0)
					result.WrongAnswers.Add(wrongAnswer);
			}

			// result
			if (result.WrongAnswers.Any())
				result.IsSuccess = false;
			else
			{
				var userCourse = await unitOfWork.UsersCoursesRepository
				.ReadAsync(uc => uc.UserId == user.Id && uc.CourseId == unit.CourseId);

				var nextUnit = await unitOfWork.UnitsRepository
					.Query
					.AsNoTracking()
					.Where(u => u.CourseId == userCourse.CourseId)
					.OrderBy(u => u.UnitOrder)
					.FirstOrDefaultAsync(u => u.UnitOrder > unit.UnitOrder);


				if (nextUnit != null)
					userCourse.CurrentUnitId = int.Max(nextUnit.UnitOrder, userCourse.CurrentUnitId);
				else if (!userCourse.PassedCourse)
				{

					userCourse.PassedCourseDate = DateTime.UtcNow;
					userCourse.PassedCourse = true;
				}

				await unitOfWork.SaveChangesAsync();
			}
			return result;
		}
	}
}
