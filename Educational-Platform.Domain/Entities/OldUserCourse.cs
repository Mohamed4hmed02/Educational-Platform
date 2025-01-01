namespace Educational_Platform.Domain.Entities
{
	public class OldUserCourse
	{
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CourseName { get; set; }
        public User? User { get; set; }
    }
}
