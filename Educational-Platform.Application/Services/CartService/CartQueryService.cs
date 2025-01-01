using Educational_Platform.Application.Abstractions.CartInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Services.CartService
{
    public class CartQueryService(IUnitOfWork unitOfWork) : ICartQueryService
    {
        public async ValueTask<QueryCartModel> GetCartAsync(object userFullId)
        {
            var user = await unitOfWork.UsersRepository
                .AsNoTracking()
                .ReadAsync(u => u.FullId.Equals(userFullId));

            var cart = await unitOfWork.CartsRepository
                .AsNoTracking()
                .ReadAsync(c => c.UserId == user.Id);

            cart.Total = await unitOfWork.CartsRepository.GetTotalPriceAsync(cart.Id);

            var model = QueryCartModel.GetModel(cart);
            return model;
        }
    }
}
