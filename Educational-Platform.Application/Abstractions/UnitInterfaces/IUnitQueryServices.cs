using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.UnitInterfaces
{
	public interface IUnitQueryServices
	{
		/// <summary>
		/// Returns a list of unit model as an object, the model type is based on <paramref name="modelType"/>
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="modelType"></param>
		/// <returns></returns>
		ValueTask<object> GetUnitsAsync(object courseId,ModelTypeEnum modelType);
		ValueTask<object> BelongToCourseAsync(object unitId);
		ValueTask<bool> CanUserAccessUnitAsync(object unitId, object userId);
	}
}
