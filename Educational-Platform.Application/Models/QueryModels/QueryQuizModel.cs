namespace Educational_Platform.Application.Models.QueryModels
{
	public class QueryQuizModel
	{
		public class QueryQuestionModel
		{
			public int Id { get; set; }
			public string Subject { get; set; } = string.Empty;
			public IEnumerable<string> Answers { get; set; } = [];
		}
		public int UnitId { get; set; }
		public IEnumerable<QueryQuestionModel> Questions { get; set; } = [];
	}
}
