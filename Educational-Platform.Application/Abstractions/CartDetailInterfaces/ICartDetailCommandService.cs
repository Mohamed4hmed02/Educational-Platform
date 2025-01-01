using Educational_Platform.Application.Models.CommandModels;

namespace Educational_Platform.Application.Abstractions.CartDetailInterfaces
{
    public interface ICartDetailCommandService
    {
        ValueTask RemoveAsync(CommandCartDetailModel model);
        ValueTask AddAsync(CommandCartDetailModel model);
    }
}
