using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Application.Abstractions.TopicInterfaces
{
	public interface ITopicCommandServices : IDeleteable<Topic>, IEditable<CommandTopicModel>
	{
		Task DeleteTopicsAsync(IEnumerable<int> topicsIds);
		ValueTask UploadPdfAsync(IFormFile file, object topicId);
		ValueTask DeletePdfAsync(object topicId);
	}
}
