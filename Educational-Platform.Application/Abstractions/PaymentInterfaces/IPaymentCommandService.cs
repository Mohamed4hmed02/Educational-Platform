namespace Educational_Platform.Application.Abstractions.PaymentInterfaces
{
    public interface IPaymentCommandService
    {
        ValueTask ConfirmByUserEmailAsync(string userEmail);
        ValueTask ConfirmAsync(object paymentId);
        /// <summary>
        /// Returns a checkout info
        /// </summary>
        /// <param name="orderFullId"></param>
        /// <returns></returns>
        ValueTask<object> CreateAsync(object orderFullId);
    }
}
