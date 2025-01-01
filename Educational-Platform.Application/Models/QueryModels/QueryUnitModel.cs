using Educational_Platform.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Educational_Platform.Application.Models.QueryModels
{
	public class QueryUnitModel
	{
		public int Id { get; set; }
		public int CourseId { get; set; }
		public string Name { get; set; } = string.Empty;
        public int NoOfTopics { get; set; }
		public IEnumerable<QueryTopicModel>? Topics { get; set; }
		public bool HasQuiz { get; set; }
		public int UnitOrder { get; set; }
	}
}
