namespace Educational_Platform.Application.Abstractions.Orders
{
    public interface IOrderCommandService
    {
        Task Make(object userFullId);
        Task Cancel(object orderFullId);
    }
}
