using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos
{
	public interface ITopicAsyncRepository : IRepositoryBase<Topic>
	{
		ValueTask<IEnumerable<QueryTopicModel>> GetTopicsModelAsync(IEnumerable<int> unitIds);
		ValueTask DeleteAsync(IEnumerable<int> topicsIds);
	}
}
