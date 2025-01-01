using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.CourseInterfaces
{
	public interface ICourseQueryServices : IPagination
	{
		ValueTask<QueryCourseDetailsModel> GetCourseInDtailAsync(object courseId);
		ValueTask<int> GetCountAsync(ModelTypeEnum modelType);
	}
}
