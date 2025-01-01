using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.TopicInterfaces
{
	public interface ITopicQueryServices
	{
		ValueTask<object> BelongToCourseAsync(object topicId);
		ValueTask<object> BelongToUnitAsync(object topicId);
		/// <summary>
		/// Returns a list of topic model as an object, the model type is based on <paramref name="modelType"/>
		/// </summary>
		/// <param name="unitId"></param>
		/// <param name="modelType"></param>
		/// <returns></returns>
		ValueTask<object> GetTopicsAsync(object unitId, ModelTypeEnum modelType);
	}
}
