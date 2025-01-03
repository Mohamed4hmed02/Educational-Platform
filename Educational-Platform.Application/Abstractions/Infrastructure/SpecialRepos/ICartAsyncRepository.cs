using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos
{
    public interface ICartAsyncRepository : IRepositoryBase<Cart>
    {
        ValueTask<decimal> GetTotalPriceAsync(object cartId);
    }
}
