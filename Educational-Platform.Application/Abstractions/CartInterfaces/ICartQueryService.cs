using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.CartInterfaces
{
    public interface ICartQueryService
    {
        ValueTask<QueryCartModel> GetCartAsync(object userFullId);
    }
}
