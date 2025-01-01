using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryOrderDetailModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImagePath { get; set; }
        public ProductTypes ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
