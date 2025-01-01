using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Domain.Entities
{
	public class CartDetail
	{
		public int CartId { get; set; }
		public int ProductId { get; set; }
		public ProductTypes ProductType { get; set; }
		public int Quantity { get; set; }
		public Cart? Cart { get; set; }
	}
}
