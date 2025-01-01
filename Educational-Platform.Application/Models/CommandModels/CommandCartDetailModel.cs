using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Models.CommandModels
{
    public class CommandCartDetailModel
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public ProductTypes ProductType { get; set; }
        public int Quantity { get; set; }

        public CartDetail GetCartDetail()
        {
            return new CartDetail
            {
                ProductType = ProductType,
                Quantity = Quantity,
                ProductId = ProductId,
                CartId = CartId
            };
        }

        public static CommandCartDetailModel GetCartDetailModel(CartDetail detail)
        {
            return new CommandCartDetailModel
            {
                ProductType = detail.ProductType,
                Quantity = detail.Quantity,
                ProductId = detail.ProductId,
                CartId = detail.CartId
            };
        }
    }
}
