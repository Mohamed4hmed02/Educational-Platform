namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data
{
	public struct Detail
	{
        public string Reference { get; set; }
    }
	public class InvoiceRequest
	{
        public required IEnumerable<Item> Items { get; set; }
        public int MyProperty { get; set; }
    }
}
