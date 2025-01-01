namespace Educational_Platform.Domain.Entities
{
	public class Unit
	{
		public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string? QuizFileName { get; set; }
        public int UnitOrder { get; set; }
        public IEnumerable<Topic>? Topics { get; set; }
		public Course? Course { get; set; }
	}
}
