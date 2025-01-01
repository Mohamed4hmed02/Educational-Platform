namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data
{
	public class OrderRequest
	{
		public string Intent { get; set; } = "CAPTURE";
        public required IEnumerable<Unit> Purchase_units { get; set; }
    }
}
