using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
	public class OldUserCoursesAsyncRepository(
		AppDbContext appDbContext,
		ILogger<AsyncRepository<OldUserCourse>> log) : AsyncRepository<OldUserCourse>(appDbContext, log), IOldUserCoursesAsyncRepository
	{
		public async ValueTask DeleteWithUserIdAsync(object userId)
		{
			string command =
				$"""
				DELETE FROM dbo.OldUserCourses
				WHERE UserId = {userId}
				""";

			await RawSqlCommandAsync(command);
		}
	}
}
