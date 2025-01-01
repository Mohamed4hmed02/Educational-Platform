using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.OrderDetails
{
    public interface IOrderDetailCommandService
    {
        Task<IEnumerable<OrderDetail>> AddItems(Order order);
    }
}
