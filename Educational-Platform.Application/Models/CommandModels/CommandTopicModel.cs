using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.CommandModels
{
	public class CommandTopicModel
	{
		public int Id { get; set; }
		public string OnHostId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int UnitId { get; set; }

		public static explicit operator CommandTopicModel(Topic topic)
		{
			return new CommandTopicModel
			{
				UnitId = topic.UnitId,
				Id = topic.Id,
				Name = topic.Name,
				OnHostId = topic.OnHostVideoId,
				Description = topic.Description
			};
		}

		public Topic GetTopic(IUnitOfWork unitOfWork)
		{
			return new Topic
			{
				UnitId = UnitId,
				Id = Id,
				Name = Name,
				OnHostVideoId = OnHostId,
				Description = Description,
				ReferenceName = GetReferenceName(unitOfWork)
			};
		}

		private string? GetReferenceName(IUnitOfWork unitOfWork)
		{
			try
			{
				return unitOfWork.TopicsRepository
					.AsNoTracking()
					.ReadAsync(t => t.Id == Id, t => t.ReferenceName)
					.Result;
			}
			catch
			{
				return null;
			}
		}
	}
}
