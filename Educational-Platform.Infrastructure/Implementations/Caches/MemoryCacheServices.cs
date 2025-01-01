using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Educational_Platform.Infrastructure.Implementations.Caches
{
    public class MemoryCacheServices(IMemoryCache cache) : ICacheServices
    {
        public void Cache(object key, object item, TimeSpan expiration)
        {
            cache.Set(key, item, expiration);
        }

        public object? GetValue(object key)
        {
            cache.TryGetValue(key, out object? value);
            return value;
        }

        public void Remove(object key)
        {
            try
            {
                cache.Remove(key);
            }
            catch
            {

            }
        }
    }
}
