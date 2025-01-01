namespace Educational_Platform.Application.Models.CommonModels
{
	public class QuizResultModel
	{
		public int UnitId { get; set; }
		public bool IsSuccess { get; set; }
		public IList<Answer> WrongAnswers { get; } = new List<Answer>();
	}
}
