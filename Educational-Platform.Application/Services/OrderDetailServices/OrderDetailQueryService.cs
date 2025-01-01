using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OrderDetails;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Services.OrderDetailServices
{
    internal class OrderDetailQueryService(
        IUnitOfWork unitOfWork,
        IStorageService storageService) : IOrderDetailQueryService
    {
        public async ValueTask<IEnumerable<QueryOrderDetailModel>> GetModels(object orderFullId)
        {
            var order = await unitOfWork.OrdersRepository
                .AsNoTracking()
                .ReadAsync(o => o.FullId.Equals(orderFullId));

            var details = unitOfWork.OrderDetailsRepository
                .Query
                .Where(od => od.OrderId == order.Id && od.ProductType == ProductTypes.Course)
                .Join(
                unitOfWork.CoursesRepository.Query,
                od => od.ProductId,
                c => c.Id,
                (od, c) => new QueryOrderDetailModel
                {
                    Price = od.Price,
                    ProductType = od.ProductType,
                    ProductId = od.ProductId,
                    ProductImagePath = c.ImageName,
                    ProductName = c.Name,
                    Quantity = od.Quantity
                }).ToList();

            foreach (var detail in details)
            {
                detail.ProductImagePath = storageService.GetFileViewPath(detail.ProductImagePath, FileTypeEnum.Image);
            }

            return details;
        }
    }
}
