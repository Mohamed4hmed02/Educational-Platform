using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Models;
using Educational_Platform.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices
{
    public abstract class PaymentGatewayAbstractService(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<CurrencyOptions> currencyOptions)
    {
        public async Task<PaymentInfoModel> GetPaymentInfoAsync(object orderId, object referenceId)
        {
            var details = await unitOfWork.OrderDetailsRepository
                .AsNoTracking()
                .ReadAllAsync(d => d.OrderId.Equals(orderId));

            var total = EGPToUSD(details.Sum(d => d.Quantity * d.Price));
            total += CalculateTaxes(total);

            return await GenerateCheckoutUrl(referenceId, details, total);
        }

        public abstract Task<PaymentStatus> GetStatusAsync(string transactionId);

        protected decimal EGPToUSD(decimal value)
        {
            return Math.Round(value / (decimal)currencyOptions.CurrentValue.EGPToUSD, 2);
        }

        protected abstract decimal CalculateTaxes(decimal total);

        protected abstract Task<PaymentInfoModel> GenerateCheckoutUrl(object referenceId, IEnumerable<OrderDetail> orderDetails, decimal total);
    }
}
