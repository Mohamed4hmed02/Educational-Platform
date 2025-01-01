using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public ProductTypes ProductType{ get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation properities
        public Order? Order { get; set; }
    }
}
