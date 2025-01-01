namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions
{
    public interface ICacheServices
    {
        void Cache(object key, object item, TimeSpan expiration);
        object? GetValue(object key);
        void Remove(object key);
    }
}
