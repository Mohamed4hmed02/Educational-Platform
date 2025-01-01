using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;
using System.Text.Json;

namespace Educational_Platform.Application.Extensions
{
	public static class StorageExtensions
	{
		public static CommandQuizModel? GetQuiz(this IStorageService storageServices, string? quizName)
		{
			var path = storageServices.GetFilePhysicalPath(quizName, FileTypeEnum.Json);
			if (path == null)
				return null;
			return JsonSerializer.Deserialize<CommandQuizModel>(File.ReadAllText(path));
		}
	}
}
