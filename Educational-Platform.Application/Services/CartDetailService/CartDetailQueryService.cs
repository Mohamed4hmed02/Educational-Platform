using Educational_Platform.Application.Abstractions.CartDetailInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Services.CartDetailService
{
    public class CartDetailQueryService(
        IUnitOfWork unitOfWork,
        IStorageService storageServices) : ICartDetailQueryService
    {
        public async ValueTask<IEnumerable<QueryCartDetailModel>> GetActiveCartDetailsAsync(object cartId)
        {
            var details = await unitOfWork.CartDetailsRepository.GetDetailsAsync(cartId);

            foreach (var detail in details)
                detail.ImageOrReferencePath = storageServices.GetFileViewPath(detail.ImageOrReferencePath, FileTypeEnum.Image) ?? string.Empty;

            return details ?? [];
        }
    }
}
