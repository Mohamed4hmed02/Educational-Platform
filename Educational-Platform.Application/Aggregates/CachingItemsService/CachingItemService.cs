using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Enums;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Aggregates.CachingItemsService
{
    public class CachingItemService(
        CachingItemStrategyFactory strategyFactory,
        ILogger<CachingItemService> logger) : ICachingItemService
    {
        private readonly static SemaphoreSlim semaphore = new(1, 1);

        public object CacheItemAndGet(CachedItemType itemType, bool isModified)
        {
            try
            {
                semaphore.Wait(TimeSpan.FromSeconds(5));
                object item = strategyFactory.GetStrategy(itemType).GetItemAndCacheAsync(isModified).Result;
                semaphore.Release();
                return item;
            }
            catch (Exception ex)
            {
                semaphore.Release();
                logger.LogError("Error while retrieving courses and books {errorMessage}", ex.Message);
                return new object();
            }

        }
    }
}
