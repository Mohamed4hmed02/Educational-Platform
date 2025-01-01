namespace Educational_Platform.Application.Abstractions.CartInterfaces
{
    public interface ICartCommandService
    {
        ValueTask<object> AddAsync(object userFullId);
        ValueTask EmptyCartAsync(object cartId);
    }
}
