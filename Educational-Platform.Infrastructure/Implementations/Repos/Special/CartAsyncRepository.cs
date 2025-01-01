using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
    public class CartAsyncRepository(
        AppDbContext appDbContext,
        ILogger<AsyncRepository<Cart>> log) : AsyncRepository<Cart>(appDbContext, log), ICartAsyncRepository
    {
        public async ValueTask<decimal> GetTotalPriceAsync(object cartId)
        {
            return
                await appDbContext.CartDetails
                .Where(c => c.CartId.Equals(cartId))
                .Join(
                appDbContext.Courses,
                cd => cd.ProductId,
                c => c.Id,
                (cd, c) => c.NetPrice)
                .SumAsync();
        }
    }
}
