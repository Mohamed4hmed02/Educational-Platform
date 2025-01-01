using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.QueryModels
{
	public class QueryTopicModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
        public int UnitId { get; set; }
		public string? ReferencePath { get; set; }

		public static explicit operator QueryTopicModel(Topic topic)
		{
			return new QueryTopicModel
			{
				Id = topic.Id,
				Name = topic.Name,
				ReferencePath = topic.ReferenceName,
				UnitId = topic.UnitId
			};
		}
	}
}
