using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.UserCourseInterfaces
{
	public interface IUserCourseQueryServices
	{
		ValueTask<IEnumerable<QueryUserCourseModel>> GetUserCoursesAsync(object userFullId);
		ValueTask<bool> CanUserAccessCourseAsync(object userId, object courseId);
		ValueTask<int> GetMaxUnitUserCanAccess(object userFullId, object courseId);
		ValueTask GetCertificateAsync(object userFullId, object courseId);
	}
}
