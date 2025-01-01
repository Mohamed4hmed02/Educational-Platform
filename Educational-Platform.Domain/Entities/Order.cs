using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string FullId { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public CurrencyTypes Currency { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = default;

        // Navigation properities
        public IEnumerable<OrderDetail>? Details { get; set; }
        public IEnumerable<Payment>? Payments { get; set; }
        public User? User { get; set; }
    }
}
