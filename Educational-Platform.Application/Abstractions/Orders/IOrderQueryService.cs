using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.Orders
{
    public interface IOrderQueryService
    {
        ValueTask<IEnumerable<QueryOrderModel>> GetModels(object userFullId);
    }
}
