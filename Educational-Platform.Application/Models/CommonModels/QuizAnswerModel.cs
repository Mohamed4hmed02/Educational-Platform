namespace Educational_Platform.Application.Models.CommonModels
{
	public struct Answer
	{
		public Answer()
		{
			ChosenAnswers = [];
		}

		public int QuestionId { get; set; }
		public IList<int> ChosenAnswers { get; set; }
	}
	public class QuizAnswerModel
	{
		public string UserId { get; set; } = string.Empty;
        public IEnumerable<Answer>? Answers { get; set; }
	}
}
