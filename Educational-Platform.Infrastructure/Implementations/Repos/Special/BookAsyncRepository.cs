using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
    public class BookAsyncRepository(
        AppDbContext appDbContext,
        ILogger<AsyncRepository<Book>> log) : AsyncRepository<Book>(appDbContext, log), IBookAsyncRepository
    {
        public async ValueTask<IEnumerable<Book>> GetBooksAsync(IEnumerable<int> Ids)
        {
            string stringIds = string.Join(',', Ids);
            if (stringIds.Length == 0)
                return [];

            string query =
                $"""
				"SELECT b.*
				"FROM dbo.Books AS b
				"WHERE b.Id IN({stringIds})
				""";

            return await RawSqlQueryAsync(query);
        }
    }
}
