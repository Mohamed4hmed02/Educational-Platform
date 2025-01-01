namespace Educational_Platform.Application.Aggregates.CachingItemsService
{
    public interface ICachingItemStrategy
    {
        Task<object> GetItemAndCacheAsync(bool isModified);
    }
}
