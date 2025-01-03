using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.TopicInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Services.TopicServices
{
    public class TopicQueryServices(
		IUnitOfWork unitOfWork,
		IStorageService storageServices) : ITopicQueryServices
	{
		public async ValueTask<object> GetTopicsAsync(object unitId, ModelTypeEnum modelType)
		{
			if (modelType == ModelTypeEnum.Query)
				return await GetQueryModels(unitId);
			else if (modelType == ModelTypeEnum.Command)
				return await GetCommandModels(unitId);
			return Array.Empty<object>();
		}

		public async ValueTask<object> BelongToCourseAsync(object topicId)
		{
			var unitId = await unitOfWork.TopicsRepository.AsNoTracking().ReadAsync(t => t.Id.Equals(topicId), t => t.UnitId);
			return await unitOfWork.UnitsRepository.AsNoTracking().ReadAsync(u => u.Id == unitId, u => u.CourseId);
		}

		public async ValueTask<object> BelongToUnitAsync(object topicId)
		{
			return await unitOfWork.TopicsRepository
				.AsNoTracking()
				.ReadAsync(
				t => t.Id.Equals(topicId),
				t => t.UnitId);
		}

		private async Task<object> GetQueryModels(object unitId)
		{
			var topics = await unitOfWork.TopicsRepository
				.AsNoTracking()
				.ReadAllAsync(t => t.UnitId.Equals(unitId), t => (QueryTopicModel)t);

			if (topics is null)
				return new object();

			var paths = storageServices.GetFilesPath(topics.Select(t => t.ReferencePath ?? "").ToArray(), FileTypeEnum.Pdf);

			for (int i = 0; i < paths?.Length; i++)
				topics.ElementAt(i).ReferencePath = paths[i];

			return topics;
		}
		private async Task<object> GetCommandModels(object unitId)
		{
			var topics = await unitOfWork.TopicsRepository.AsNoTracking()
				.ReadAllAsync(t => t.UnitId.Equals(unitId), t => (CommandTopicModel)t);

			return topics;
		}
	}
}
