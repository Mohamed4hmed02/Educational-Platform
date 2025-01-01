using System.Text.Json;
using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
    internal class SqlQueryModelAdapter : QueryUnitModel
    {
        public new string? Topics { get; set; }
        public QueryUnitModel GetQueryUnitModel(SqlQueryModelAdapter temp)
        {
            var model = new QueryUnitModel
            {
                CourseId = temp.CourseId,
                Id = temp.Id,
                Name = temp.Name,
                NoOfTopics = temp.NoOfTopics,
                HasQuiz = temp.HasQuiz,
                UnitOrder = temp.UnitOrder
            };
            if (temp.Topics is not null)
                model.Topics = JsonSerializer.Deserialize<ICollection<QueryTopicModel>>(temp.Topics);
            return model;
        }
    }

    public class UnitAsyncRepository(
        AppDbContext appDbContext,
        ILogger<AsyncRepository<Unit>> log) : AsyncRepository<Unit>(appDbContext, log), IUnitAsyncRepository
    {
        public async ValueTask DeleteAsync(IEnumerable<int> Ids)
        {
            string stringIds = string.Join(',', Ids);

            if (stringIds.Length == 0)
                return;

            string deleteCommand =
                $"""
				DELETE FROM dbo.Units
				WHERE Id IN({stringIds})
				""";
            await RawSqlCommandAsync(deleteCommand);
        }

        public async ValueTask<IEnumerable<int>> GetTopicsIdAsync(IEnumerable<int> unitsId)
        {
            var stringIds = string.Join(',', unitsId);

            if (stringIds.Length == 0)
                return [];

            string retrieveQuery =
                $"""
				SELECT Id
				FROM dbo.Topics
				WHERE UnitId IN({stringIds})
				""";

            return await RawSqlQueryAsync<int>(retrieveQuery);
        }

        public async ValueTask<IEnumerable<QueryUnitModel>> GetUnitsModelAsync(object? courseId = null)
        {
            string subQuery1 =
                """
				SELECT t.Id,t.Name,t.UnitId,t.ReferenceName AS ReferencePath
				FROM dbo.Topics AS t
				WHERE t.UnitId = u.Id
				FOR JSON PATH
				""";

            string subQuery2 =
                """
				SELECT
				CASE
				WHEN u2.QuizFileName IS NULL THEN CONVERT(BIT,0)
				ELSE CONVERT(BIT,1)
				END
				FROM dbo.Units AS u2
				WHERE u2.Id = u.Id
				""";

            string subQuery3 =
                """
				SELECT UnitOrder
				FROM dbo.Units AS u2
				WHERE u2.Id = u.Id
				""";

            string subQuery4 =
                """
				SELECT COUNT(t.UnitId)
				FROM dbo.Topics AS t
				WHERE t.UnitId = u.Id
				""";

            string query =
                $"""
				SELECT u.Id,u.CourseId,u.Name,
				({subQuery4}) AS NoOfTopics,
				({subQuery1}) AS Topics,
				({subQuery2}) AS HasQuiz,
				({subQuery3}) AS UnitOrder
				FROM dbo.Units AS u
				WHERE CourseId = {courseId}
				""";

            if (courseId is null)
                query =
                $"""
				SELECT u.Id,u.CourseId,u.Name,
				({subQuery4}) AS NoOfTopics,
				({subQuery1}) AS Topics,
				({subQuery2}) AS HasQuiz,
				({subQuery3}) AS UnitOrder
				FROM dbo.Units AS u
				""";

            return (await RawSqlQueryAsync<SqlQueryModelAdapter>(query)).Select(u => u.GetQueryUnitModel(u));
        }
    }
}
