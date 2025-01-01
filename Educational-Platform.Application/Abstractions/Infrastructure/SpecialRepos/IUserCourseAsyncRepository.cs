using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos
{
    public interface IUserCourseAsyncRepository : IAsyncRepositoryBase<UserCourse>
    {
        ValueTask<IEnumerable<int>> GetExistIdsAsync(IEnumerable<int> Ids);
		ValueTask AddCoursesToUserAsync(IEnumerable<CommandUserCourseModel> models);
	}
}
