namespace Educational_Platform.Application.Models.CommandModels
{
	public class CommandUserCourseModel
	{
		public int CourseId { get; set; }
		public string UserFullIdOrEmail { get; set; } = string.Empty;
    }
}
