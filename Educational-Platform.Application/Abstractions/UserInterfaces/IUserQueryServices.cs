using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.UserInterfaces
{
	public interface IUserQueryServices
	{
		ValueTask<QueryUserModel> GetUserAsync(object entityId);
	}
}
