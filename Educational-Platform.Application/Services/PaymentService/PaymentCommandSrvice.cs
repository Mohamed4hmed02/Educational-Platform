using Educational_Platform.Application.Abstractions.BookInterfaces;
using Educational_Platform.Application.Abstractions.CartInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.PaymentInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.PaymentService
{
    public partial class PaymentCommandSrvice(
        IUnitOfWork unitOfWork,
        PaymentGatewayAbstractService paymentGateway,
        ICartCommandService cartCommand,
        ILogger<PaymentCommandSrvice> log) : IPaymentCommandService
    {
        public async ValueTask ConfirmAsync(object paymentId)
        {
            var payment = await unitOfWork.PaymentsRepository
                .ReadAsync(p => p.Id.Equals(paymentId) && p.Status == PaymentStatus.Authorized);

            var order = await unitOfWork.OrdersRepository
                .ReadAsync(o => o.Id == payment.OrderId);

            await Confirm(order, payment);
        }

        public async ValueTask ConfirmByUserEmailAsync(string userEmail)
        {
            var user = await unitOfWork.UsersRepository
                .AsNoTracking()
                .ReadAsync(u => u.Email == userEmail);

            var order = await unitOfWork.OrdersRepository
                .ReadAsync(o => o.UserId == user.Id);

            var payment = await unitOfWork.PaymentsRepository
                .ReadAsync(p => p.OrderId == order.Id);

            await Confirm(order, payment);
        }

        public async ValueTask<object> CreateAsync(object orderFullId)
        {
            var order = await unitOfWork.OrdersRepository
                .AsNoTracking()
                .ReadAsync(o => o.FullId.Equals(orderFullId) && o.Status != OrderStatus.Cancelled);

            var fullId = GeneratePaymentFullId();
            var gatewayInfo = await paymentGateway.GetPaymentInfoAsync(order.Id, fullId);
            var payment = new Payment
            {
                OrderId = order.Id,
                Status = PaymentStatus.Authorized,
                Total = order.Total,
                FullId = fullId,
                TransactionId = gatewayInfo.PaymentId
            };

            await unitOfWork.PaymentsRepository.CreateAsync(payment);
            await unitOfWork.SaveChangesAsync();
            return new { gatewayInfo.Link };
        }
    }

    public partial class PaymentCommandSrvice
    {
        private string GeneratePaymentFullId()
        {
            return Guid.NewGuid().ToString("N").ToUpper()[0..8];
        }

        private async ValueTask AddCoursesToUserAsync(IEnumerable<OrderDetail> details, int? userId)
        {
            var user = await unitOfWork.UsersRepository
                .AsNoTracking()
                .ReadAsync(u => u.Id == userId);

            var courses = details.Where(d => d.ProductType == ProductTypes.Course);
            var userCourses = courses.Select(c => new CommandUserCourseModel
            {
                CourseId = c.ProductId,
                UserFullIdOrEmail = user.FullId
            });

            await unitOfWork.UsersCoursesRepository.AddCoursesToUserAsync(userCourses);
        }

        private async Task Confirm(Order order, Payment payment)
        {
            var details = await unitOfWork.OrderDetailsRepository
                .AsNoTracking()
                .ReadAllAsync(d => d.OrderId == order.Id);
       
            await AddCoursesToUserAsync(details, order.UserId);

            payment.Status = PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;
            payment.Total = order.Total;

            order.Status = OrderStatus.Completed;

            await unitOfWork.SaveChangesAsync();

            var cart = await unitOfWork.CartsRepository
                .AsNoTracking()
                .ReadAsync(c => c.UserId == order.UserId);

            await cartCommand.EmptyCartAsync(cart.Id);

            log.LogInformation("A Payemnt WithId {PayemntId} Is Confirmed", payment.Id);
        }
    }
}
