using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.CommonModels
{
	public class OldUserCourseModel
	{
		public string CourseName { get; set; } = string.Empty;
        public static explicit operator OldUserCourseModel(OldUserCourse course)
		{
			return new OldUserCourseModel
			{
				CourseName = course.CourseName
			};
		}

	}
}
