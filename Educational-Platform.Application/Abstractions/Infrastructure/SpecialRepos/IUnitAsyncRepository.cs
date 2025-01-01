using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos
{
    public interface IUnitAsyncRepository : IAsyncRepositoryBase<Unit>
    {
        /// <summary>
        /// If course id is null it will return the whole units table
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        ValueTask<IEnumerable<QueryUnitModel>> GetUnitsModelAsync(object? courseId = null);
        ValueTask DeleteAsync(IEnumerable<int> Ids);
        ValueTask<IEnumerable<int>> GetTopicsIdAsync(IEnumerable<int> unitsId);
    }
}
