namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices
{
	/// <summary>
	/// Implement a method that continously check for approved orders to complete thier payments
	/// </summary>
	public interface IApprovePaymentOrder
	{
		Task ApproveAsync();
	}
}
