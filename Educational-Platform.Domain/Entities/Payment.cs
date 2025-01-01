using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string FullId { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Authorized;
        public DateTime PaidAt { get; set; } = default;
        public decimal Total { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
    }
}
