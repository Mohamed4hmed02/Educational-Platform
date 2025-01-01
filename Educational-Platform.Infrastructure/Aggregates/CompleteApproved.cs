using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.PaymentInterfaces;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Aggregates
{
    internal class CompleteApproved(
        IUnitOfWork unitOfWork,
        IPaymentCommandService paymentCommand,
        PaymentGatewayAbstractService paymentGateway,
        ILogger<BackGroundServices> log) : IApprovePaymentOrder
    {
        private readonly static SemaphoreSlim _semaphore = new(1, 1);
        private static int currentOFFSET = 0;
        private readonly int _noOfRows = 10;

        public async Task ApproveAsync()
        {
            _semaphore.Wait(TimeSpan.FromSeconds(10));

            var payments = await unitOfWork.PaymentsRepository
                .Query
                .Where(p =>
                (p.Status != PaymentStatus.Completed) &&
                (p.Status != PaymentStatus.Expired))
                .OrderBy(p => p.Id)
                .Skip(currentOFFSET)
                .Take(_noOfRows)
                .ToArrayAsync();

            if (!payments.IsNullOrEmpty())
            {
                foreach (var payment in payments)
                {
                    var status = await paymentGateway.GetStatusAsync(payment.TransactionId);
                    if ((status == PaymentStatus.Captured) || (status == PaymentStatus.Completed))
                        await ApproveAsync(payment.Id);

                    else if (status == PaymentStatus.Expired)
                        await DeleteExpiredAsync(payment);
                }

                currentOFFSET += _noOfRows;
            }
            else
                currentOFFSET = 0;

            _semaphore.Release();
        }

        private async Task ApproveAsync(int paymentId)
        {
            try
            {
                await paymentCommand.ConfirmAsync(paymentId);
            }
            catch (Exception ex)
            {
                log.LogError("An Error Occurred While Confirm A Payment {fullId}, {errorMessage}", paymentId, ex.Message);
                return;
            }
        }

        private async Task DeleteExpiredAsync(Payment payment)
        {
            payment.Status = PaymentStatus.Expired;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
