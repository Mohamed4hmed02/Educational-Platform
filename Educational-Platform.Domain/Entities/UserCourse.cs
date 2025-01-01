namespace Educational_Platform.Domain.Entities
{
	public class UserCourse
	{
		public int UserId { get; set; }
		public int CourseId { get; set; }
		public DateTime StartDate { get; set; }
		public int CurrentUnitId { get; set; }
        public bool PassedCourse { get; set; }=false;
        public DateTime PassedCourseDate { get; set; }
        public User? User { get; set; }
		public Course? Course { get; set; }
	}
}
