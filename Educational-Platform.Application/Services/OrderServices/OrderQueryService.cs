using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.Orders;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Services.OrderServices
{
    internal class OrderQueryService(IUnitOfWork unitOfWork) : IOrderQueryService
    {
        public async ValueTask<IEnumerable<QueryOrderModel>> GetModels(object userFullId)
        {
            var user = await unitOfWork.UsersRepository
                .AsNoTracking()
                .ReadAsync(u => u.FullId.Equals(userFullId));

            return await unitOfWork.OrdersRepository
                .AsNoTracking()
                .ReadAllAsync(o =>
                o.UserId == user.Id,
                o => QueryOrderModel.GetModel(o));
        }
    }
}
