using Educational_Platform.Application.Abstractions.CartInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.CartService
{
    public partial class CartCommandService(
        IUnitOfWork unitOfWork,
        ILogger<CartCommandService> log) : ICartCommandService
    {
        public async ValueTask<object> AddAsync(object userFullId)
        {
            var user = await unitOfWork.UsersRepository
                .AsNoTracking()
                .ReadAsync(
                    u => userFullId.Equals(u.FullId),
                    u => new
                    {
                        u.Id
                    }
                );

            Cart? cart = default;
            try
            {
                cart = await unitOfWork.CartsRepository
                    .AsNoTracking()
                    .ReadAsync(c => c.UserId == user.Id);

                return cart.Id;
            }
            catch
            {
                cart = new Cart
                {
                    UserId = user.Id
                };

                await unitOfWork.CartsRepository.CreateAsync(cart);
                await unitOfWork.SaveChangesAsync();

                log.LogInformation("A New Cart WithId {CartId} Is Added", cart.UserId);

                return cart.Id;
            }
        }

        public async ValueTask EmptyCartAsync(object cartId)
        {
            var details = await unitOfWork.CartDetailsRepository
                .ReadAllAsync(d => d.CartId.Equals(cartId));

            await unitOfWork.CartDetailsRepository.DeleteAsync(details);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
