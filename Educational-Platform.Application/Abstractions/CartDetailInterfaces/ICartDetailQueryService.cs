using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.CartDetailInterfaces
{
    public interface ICartDetailQueryService
    {
        ValueTask<IEnumerable<QueryCartDetailModel>> GetActiveCartDetailsAsync(object cartId);
    }
}
