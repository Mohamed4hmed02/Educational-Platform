using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Aggregates.CachingItemsService.Strategies;
using Educational_Platform.Application.Enums;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;

namespace Educational_Platform.Application.Aggregates.CachingItemsService
{
    public class CachingItemStrategyFactory(
        IUnitOfWork unitOfWork,
        ICacheServices cacheServices,
        IStorageService storageServices)
    {
        public ICachingItemStrategy GetStrategy(CachedItemType itemType)
        {
            if (itemType == CachedItemType.Course)
                return new CourseCachingItemStrategy(unitOfWork, cacheServices, storageServices);

            else if (itemType == CachedItemType.Book)
                return new BookCachingItemStrategy(unitOfWork, cacheServices, storageServices);

            else if (itemType == CachedItemType.Unit)
                return new UnitCachingItemStrategy(unitOfWork, cacheServices, storageServices);

            return new NullCachingItemStrategy();
        }
    }
}
