namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data
{
	public struct Link
	{
		public required string Href { get; set; }
		public required string Rel { get; set; }
		public string Method { get; set; }
	}
	public static class PaypalOrderStatus
	{
		public static string Created { get; } = "CREATED";
		public static string Saved { get; } = "SAVED";
		public static string Approved { get; } = "APPROVED";
		public static string Voided { get; } = "VOIDED";
		public static string Completed { get; } = "COMPLETED";
		public static string Payer_Action_Required { get; } = "PAYER_ACTION_REQUIRED";

	}
	public class Order
	{
		public string Created_time { get; set; }
		public string Update_time { get; set; }
		public string Id { get; set; }
		public IEnumerable<Unit> Purchase_units { get; set; }
		public IEnumerable<Link> Links { get; set; }
		public string Status { get; set; }
	}
}
