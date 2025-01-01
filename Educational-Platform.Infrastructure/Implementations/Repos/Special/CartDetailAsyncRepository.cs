using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Repos.Special
{
    public class CartDetailAsyncRepository(
        AppDbContext appDbContext,
        ILogger<AsyncRepository<CartDetail>> log) : AsyncRepository<CartDetail>(appDbContext, log), ICartDetailAsyncRepository
    {
        public async ValueTask<IEnumerable<QueryCartDetailModel>> GetDetailsAsync(object cartId)
        {
            return await Query
                .Where(c => c.CartId.Equals(cartId))
                .Join(
                appDbContext.Courses,
                cd => cd.ProductId,
                c => c.Id,
                (cd, c) => new QueryCartDetailModel
                {
                    ImageOrReferencePath = c.ImageName ?? "",
                    ProductName = c.Name,
                    ProductPrice = c.NetPrice,
                    ProductQuantity = cd.Quantity,
                    ProductType = Domain.Enums.ProductTypes.Course
                }).ToArrayAsync();
        }
    }
}
