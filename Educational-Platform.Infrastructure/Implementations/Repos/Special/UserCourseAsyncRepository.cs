using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
    public class UserCourseAsyncRepository(
		AppDbContext appDbContext,
		IRepositoryBase<User> userRepo,
		ILogger<AsyncRepository<UserCourse>> log) : AsyncRepository<UserCourse>(appDbContext, log), IUserCourseAsyncRepository
	{
		public async ValueTask AddCoursesToUserAsync(IEnumerable<CommandUserCourseModel> models)
		{
			if (!models.Any())
				return;

			var existIds = await GetExistIdsAsync(models.Select(m => m.CourseId));

			if (existIds.Count() != models.Count())
			{
				log.LogError("Exist Ids {@existIds}, Need To Add Ids {@needIds}", existIds, models.Select(m => m.CourseId));
				throw new NotExistException("Some Course Id Is Not Exist");
			}

			var userId = await userRepo
				.AsNoTracking()
				.ReadAsync(u => models.ElementAt(0).UserFullIdOrEmail.Equals(u.FullId),
				u => u.Id);

			var userCourses = models.Select(m => new UserCourse
			{
				CourseId = m.CourseId,
				UserId = userId,
				StartDate = DateTime.UtcNow,
				CurrentUnitId = 1
			});

			await CreateAsync(userCourses);
			try
			{
				await _appDbContext.SaveChangesAsync();
				foreach (var model in models)
				{
					log.LogInformation("An User WithId {UserId} Has Given Access To Course WithId {UnitId}", model.UserFullIdOrEmail, model.CourseId);
				}
			}
			catch
			{
				log.LogError("User Already Has Access To Some Course");
				throw new DuplicateNameException("User Already Has Access To Some Course")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);
			}
		}

		public async ValueTask<IEnumerable<int>> GetExistIdsAsync(IEnumerable<int> Ids)
		{
			var stringIds = string.Join(',', Ids);

			return await RawSqlQueryAsync<int>(
				$"""
				SELECT c.Id
				FROM dbo.Courses AS c
				WHERE Id IN({stringIds})
				"""
			);
		}
	}
}
