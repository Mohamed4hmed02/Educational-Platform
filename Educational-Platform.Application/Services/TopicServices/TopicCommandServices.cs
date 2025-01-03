using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.TopicInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.TopicServices
{
    public class TopicCommandServices(
		IUnitOfWork unitOfWork,
		IVideoHostServices hostServices,
		IStorageService storageServices,
		ILogger<TopicCommandServices> log) : ITopicCommandServices
	{
		public async ValueTask CreateAsync(CommandTopicModel entity)
		{
			if (entity.Id != 0)
				throw new InvalidDataException("Id Must Be Zero To Add A New Topic")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

			await ValidateTopicAsync(entity.GetTopic(unitOfWork));

			var video = await unitOfWork.TopicsRepository.CreateAsync(entity.GetTopic(unitOfWork));

			await unitOfWork.SaveChangesAsync();
			await hostServices.UpdateVideoTitleAsync(video.Id, video.Name);
			await hostServices.SetVideoPrivacyAsync(video.Id);

			log.LogInformation("A New Topic {@Topic} Was Added", (CommandTopicModel)video);
		}

		public async ValueTask UpdateAsync(CommandTopicModel entity)
		{
			if (!await unitOfWork.TopicsRepository.IsExistAsync(v => v.Id == entity.Id))
				throw new NotExistException("Topic Is Not Exist");

			Topic topic = entity.GetTopic(unitOfWork);

			await ValidateTopicAsync(topic);

			await unitOfWork.TopicsRepository.UpdateAsync(topic);
			await unitOfWork.SaveChangesAsync();

			_ = hostServices.UpdateVideoTitleAsync(topic.Id, entity.Name);

			log.LogInformation("A Topic Was Updated To {@NewTopic}", entity);
		}

		public async ValueTask DeleteAsync(object Id)
		{
			var video = await unitOfWork.TopicsRepository.DeleteAsync(Id);
			await DeleteVideoFromHostAsync(video);

			await unitOfWork.SaveChangesAsync();

			log.LogWarning("A Topic {@Topic} Was Deleted In Both DataBase And Topic Host Platform", (CommandTopicModel)video);
		}

		public async Task DeleteTopicsAsync(IEnumerable<int> topicsIds)
		{
			if (!topicsIds.Any())
				return;

			foreach (var id in topicsIds)
			{
				await hostServices.DeleteAsync(id);
				await unitOfWork.TopicsRepository.DeleteAsync(id);
				await unitOfWork.SaveChangesAsync();
			}

			log.LogWarning("All Videos Were Deleted Successfully");
		}

		public async ValueTask UploadPdfAsync(IFormFile file, object topicId)
		{
			var topic = await unitOfWork.TopicsRepository.ReadAsync(topicId);
			var pathName = await storageServices.SaveFileAsync(file, $"T{topicId}-Ref", FileTypeEnum.Pdf);
			topic.ReferenceName = pathName;
			await unitOfWork.SaveChangesAsync();
			log.LogInformation("A Reference Is Added To Topic {@Topic}", (CommandTopicModel)topic);
		}

		public async ValueTask DeletePdfAsync(object topicId)
		{
			var topic = await unitOfWork.TopicsRepository.ReadAsync(topicId);
			await storageServices.DeleteFileAsync(topic.ReferenceName, FileTypeEnum.Pdf);
			topic.ReferenceName = null;
			await unitOfWork.SaveChangesAsync();
			log.LogInformation("A Reference Is Deleted From Topic {@Topic}", (CommandTopicModel)topic);
		}

		private async ValueTask DeleteVideoFromHostAsync(Topic topic)
		{
			try
			{
				await hostServices.DeleteAsync(topic.Id);
			}
			catch (Exception ex)
			{
				log.LogError("A Topic {@Topic} Was Deleted But There Is An Error In Topic Host Platform : {ErrorMessage}", (CommandTopicModel)topic, ex.Message);
				throw new OuterServiceException($"A Topic With Id:{topic.Id} Was Deleted But There Is An Error In Topic Host Platform : {ex.Message}")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
			}
		}

		private async ValueTask ValidateTopicAsync(Topic topic)
		{
			if (!await unitOfWork.UnitsRepository.IsExistAsync(u => u.Id == topic.UnitId))
				throw new NotExistException("Unit Is Not Exist");

			if (await unitOfWork.TopicsRepository.IsExistAsync(t =>
			t.UnitId == topic.UnitId &&
			t.Name == topic.Name &&
			t.Id != topic.Id))
				throw new InvalidOperationException("There is a topic with same name within this unit")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);
		}
	}
}
