namespace Educational_Platform.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public int? IntroTopicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageName { get; set; }
        public bool IsVisible { get; set; } = false;
        public float Discount { get; set; }
        public DateOnly DiscountEndTime { get; set; }
        public bool IsDiscountAvailable => DateOnly.FromDateTime(DateTime.UtcNow) <= DiscountEndTime;
        public decimal NetPrice { get; set; }

        // Navgiation Properties
        public IEnumerable<Unit>? Units { get; set; }
        public IEnumerable<UserCourse>? Users { get; set; }
    }
}
