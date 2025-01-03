using Educational_Platform.Application.Abstractions.BookInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OrderDetails;
using Educational_Platform.Application.Abstractions.Orders;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.OrderServices
{
    internal class OrderCommandService(
        IUnitOfWork unitOfWork,
        IBookCommandServices bookCommandServices,
        IOrderDetailCommandService orderDetailCommand,
        ILogger<OrderCommandService> logger) : IOrderCommandService
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public async Task Make(object userFullId)
        {
            var user = await unitOfWork.UsersRepository
                .AsNoTracking()
                .ReadAsync(u => u.FullId.Equals(userFullId));

            if (await unitOfWork.OrdersRepository.IsExistAsync(o =>
            o.UserId == user.Id && o.Status == OrderStatus.Pending))
            {
                throw new InvalidOperationException("Already have an active order")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status423Locked);
            }

            _semaphoreSlim.Wait(TimeSpan.FromSeconds(10));

            var cart = await unitOfWork.CartsRepository
                .AsNoTracking()
                .ReadAsync(c => c.UserId == user.Id);

            var total = await unitOfWork.CartsRepository.GetTotalPriceAsync(cart.Id);

            var order = new Order
            {
                FullId = GenerateFullId(),
                Currency = CurrencyTypes.EGP,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                Total = total
            };

            await unitOfWork.OrdersRepository.CreateAsync(order);
            await unitOfWork.SaveChangesAsync();

            var details = await orderDetailCommand.AddItems(order);
            //await UpdateBooksQuantityAsync(details);

            logger.LogInformation("A new order is placed with id {id}", order.FullId);
            _semaphoreSlim.Release();
        }

        public async Task Cancel(object orderFullId)
        {
            var order = await unitOfWork.OrdersRepository
                .ReadAsync(o => o.FullId.Equals(orderFullId));
            order.Status = OrderStatus.Cancelled;
            await unitOfWork.SaveChangesAsync();
        }

        private string GenerateFullId()
        {
            return Guid.NewGuid().ToString("N").ToUpper()[0..8];
        }

        private async ValueTask UpdateBooksQuantityAsync(IEnumerable<OrderDetail> details)
        {
            var books = details.Where(d => d.ProductType == ProductTypes.Book);
            var booksIds = books.Select(b => b.ProductId);
            var quantities = books.Select(b => b.Quantity);

            await bookCommandServices.UpdateBooksQuantityAsync(booksIds, quantities);
        }
    }
}
