using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OrderDetails;
using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.OrderDetailServices
{
    internal class OrderDetailCommandService(
        IUnitOfWork unitOfWork,
        ILogger<OrderDetailCommandService> logger) : IOrderDetailCommandService
    {
        public async Task<IEnumerable<OrderDetail>> AddItems(Order order)
        {
            var details = await unitOfWork.CartDetailsRepository
                .Query
                .Join(
                unitOfWork.CoursesRepository.Query,
                cd => cd.ProductId,
                c => c.Id,
                (cd, c) => new OrderDetail
                {
                    OrderId = order.Id,
                    Price = c.NetPrice,
                    ProductId = cd.ProductId,
                    ProductType = cd.ProductType,
                    Quantity = cd.Quantity
                }).ToArrayAsync();

            await unitOfWork.OrderDetailsRepository.CreateAsync(details);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Items are added to the order with id {} successfully", order.Id);

            return details;
        }
    }
}
