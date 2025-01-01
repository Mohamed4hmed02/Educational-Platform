namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data
{
    public struct Tax
    {
        public required string Name { get; set; }
        public string? Tax_note { get; set; }
		public required string Percent { get; set; }

	}
    public struct Discount
    {
        public string Percent { get; set; }
        public Amount Amount { get; set; }
    }
	public class Item
	{
        public required string Name { get; set; }
        public required string Quantity { get; set; }
        public string? Description { get; set; }
        public string? Sku { get; set; }
        public string? Url { get; set; }
        public string? Category { get; set; }
        public required Amount Unit_amount { get; set; }
        public Tax? Tax { get; set; }
        public Discount? Discount { get; set; }
	}
}
