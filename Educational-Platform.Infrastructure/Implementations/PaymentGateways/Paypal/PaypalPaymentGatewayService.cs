using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data;
using Educational_Platform.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Unit = Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data.Unit;

namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal
{
    internal class PaypalPaymentGatewayService(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<CurrencyOptions> currencyOptions,
        PaypalPaymentGatewayServiceBase serviceBase) : PaymentGatewayAbstractService(unitOfWork, currencyOptions)
    {
        public async override Task<PaymentStatus> GetStatusAsync(string transactionId)
        {
            var order = await serviceBase.GetOrderDetailsAsync(transactionId);

            if (order.Status == PaypalOrderStatus.Approved)
                return PaymentStatus.Captured;

            if (order.Status == PaypalOrderStatus.Completed)
                return PaymentStatus.Completed;

            return PaymentStatus.Expired;
        }

        protected override decimal CalculateTaxes(decimal total)
        {
            return 0;
        }

        protected async override Task<PaymentInfoModel> GenerateCheckoutUrl(object referenceId, IEnumerable<OrderDetail> orderDetails, decimal total)
        {
            var orderItems = orderDetails.Select(d =>
                new Item
                {
                    Name = d.ProductId.ToString() ?? string.Empty,
                    Quantity = d.Quantity.ToString(),
                    Unit_amount = new Amount
                    {
                        Currency_code = "USD",
                        Value = total.ToString()
                    },
                    Sku = d.ProductId.ToString(),
                    Category = d.ProductType.ToString()
                }
            );

            var paypalReferenceId = Convert.ToString(referenceId) ?? throw new ArgumentNullException();

            var unit = new Unit
            {
                Amount = new Amount
                {
                    Currency_code = "USD",
                    Value = total.ToString()
                },
                Reference_Id = paypalReferenceId
            };

            var orderRequest = new OrderRequest
            {
                Purchase_units = [unit]
            };

            var response = await serviceBase.CreateOrderAsync(orderRequest, paypalReferenceId);

            return new PaymentInfoModel
            {
                Link = response.Links.FirstOrDefault(f => f.Rel == "approve").Href,
                PaymentId = response.Id
            };
        }
    }
}
