namespace Educational_Platform.Application.Aggregates.CachingItemsService.Strategies
{
    public class NullCachingItemStrategy : ICachingItemStrategy
    {
        public Task<object> GetItemAndCacheAsync(bool isModified)
        {
            return Task.FromResult(new object());
        }
    }
}
