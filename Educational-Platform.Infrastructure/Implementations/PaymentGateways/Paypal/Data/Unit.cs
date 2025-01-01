namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data
{
	public struct Amount
	{
		/// <summary>
		/// The three-character ISO-4217 currency code that identifies the currency.
		/// </summary>
		public required string Currency_code { get; set; }
		/// <summary>
		/// The value, which might be:
		///An integer for currencies like JPY that are not typically fractional.
		///A decimal fraction for currencies like TND that are subdivided into thousandths.
		/// </summary>
		public string Value { get; set; }
    }
	public class Unit
	{
        public required string Reference_Id { get; set; }
        public string? Description { get; set; }
        public string? Custom_Id { get; set; }
        public string? Invoice_Id { get; set; }
        public string? Soft_Descriptor { get; set; }
        public required Amount Amount { get; set; }
        public IEnumerable<Item>? Items { get; set; }
    }
}
