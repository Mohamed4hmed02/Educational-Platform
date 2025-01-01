using Educational_Platform.Application.Models.CommandModels;

namespace Educational_Platform.Application.Abstractions.UserCourseInterfaces
{
	public interface IUserCourseCommandServices
	{
		ValueTask AddCourseToUserAsync(CommandUserCourseModel model);
	}
}
