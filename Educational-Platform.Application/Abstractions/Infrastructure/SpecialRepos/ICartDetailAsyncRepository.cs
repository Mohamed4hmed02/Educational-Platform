using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos
{
    public interface ICartDetailAsyncRepository : IRepositoryBase<CartDetail>
    {
        ValueTask<IEnumerable<QueryCartDetailModel>> GetDetailsAsync(object cartId);
    }
}
