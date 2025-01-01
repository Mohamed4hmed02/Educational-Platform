using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Models.CommandModels
{
    public class CommandQuizModel
    {
        public class CommandQuestionModel
        {
            public int Id { get; set; }
            public string Subject { get; set; } = string.Empty;
            public IEnumerable<string> Answers { get; set; } = [];
            public IEnumerable<int> CorrectAnswers { set; get; } = [];
        }
        public int UnitId { get; set; }
        public IEnumerable<CommandQuestionModel> Questions { get; set; } = [];
        public static explicit operator QueryQuizModel?(CommandQuizModel? command)
        {
            if (command == null)
                return null;

            return new QueryQuizModel
            {
                UnitId = command.UnitId,
                Questions = command.Questions.Select(q => new QueryQuizModel.QueryQuestionModel
                {
                    Id = q.Id,
                    Subject = q.Subject,
                    Answers = q.Answers
                })
            };
        }
    }
}
