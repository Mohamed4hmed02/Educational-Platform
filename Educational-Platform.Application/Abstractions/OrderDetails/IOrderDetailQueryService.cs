using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.OrderDetails
{
    public interface IOrderDetailQueryService
    {
        ValueTask<IEnumerable<QueryOrderDetailModel>> GetModels(object orderFullId);
    }
}
