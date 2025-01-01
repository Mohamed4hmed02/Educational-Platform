namespace Educational_Platform.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal Total { get; set; }


        // Navigation properties
        public User? User { get; set; }
        public IEnumerable<CartDetail>? CartDetails { get; set; }
    }
}
