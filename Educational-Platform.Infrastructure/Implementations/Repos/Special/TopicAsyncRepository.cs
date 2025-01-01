using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
	public class TopicAsyncRepository(
		AppDbContext appDbContext,
		ILogger<AsyncRepository<Topic>> log) : AsyncRepository<Topic>(appDbContext, log), ITopicAsyncRepository
	{
		public async ValueTask DeleteAsync(IEnumerable<int> topicsIds)
		{
			var ids = string.Join(",", topicsIds);
			
			if (ids.Length == 0)
				return;
			
			string sqlCommand =
			   $"""
				DELETE FROM dbo.Topics
				WHERE Id IN({ids})
				""";

			await RawSqlCommandAsync(sqlCommand);
		}

		public async ValueTask<IEnumerable<QueryTopicModel>> GetTopicsModelAsync(IEnumerable<int> unitIds)
		{
			string stringIds = string.Join(',', unitIds);
			if (stringIds.Length == 0)
				return [];

			string query =
				$"""
				SELECT t.Id,t.Name,t.UnitId,t.ReferenceName AS ReferencePath
				FROM dbo.Topics AS t
				WHERE t.Id IN ({stringIds})
				ORDER BY t.Id		
				""";
			return await RawSqlQueryAsync<QueryTopicModel>(query);
		}
	}
}
