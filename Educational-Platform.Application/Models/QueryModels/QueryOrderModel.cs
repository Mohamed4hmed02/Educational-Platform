using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryOrderModel
    {
        public string Id { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static QueryOrderModel GetModel(Order order)
        {
            return new QueryOrderModel
            {
                Id = order.FullId,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Currency = order.Currency.ToString(),
                Status = order.Status.ToString(),
                Total = order.Total
            };
        }
    }
}
