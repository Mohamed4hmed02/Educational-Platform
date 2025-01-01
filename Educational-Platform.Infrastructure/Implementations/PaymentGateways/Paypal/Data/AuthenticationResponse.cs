namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data
{
	public class AuthenticationResponse
	{
		public string Scope { get; set; }
		public string Access_Token { get; set; }
		public string Token_Type { get; set; }
		public string App_Id { get; set; }
		public int Expires_In { get; set; }
		public string Nonce { get; set; }
	}
}
