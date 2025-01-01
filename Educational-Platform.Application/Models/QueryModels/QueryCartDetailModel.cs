using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryCartDetailModel
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public ProductTypes ProductType { get; set; }
        public string ImageOrReferencePath { get; set; } = string.Empty;
    }
}
