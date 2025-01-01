namespace Educational_Platform.Domain.Entities
{
	public class Topic
	{
		public int Id { get; set; }
		public int UnitId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string? ReferenceName { get; set; }
		public string OnHostVideoId { get; set; }
		public Unit? Unit { get; set; }
	}
}
