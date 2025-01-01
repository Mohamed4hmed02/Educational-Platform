namespace Educational_Platform.Application.Models.QueryModels
{
	public class QueryUserCourseModel
	{
		public int CourseId { get; set; }
		public string UserFullId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
		public int MaxUnitUserCanAccess { get; set; }
		public bool PassedCourse { get; set; }
		public DateTime PassedCourseDate { get; set; }
	}
}
