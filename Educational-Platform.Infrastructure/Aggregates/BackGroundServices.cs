using Hangfire;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Educational_Platform.Infrastructure.Aggregates
{
	public class BackGroundServices : IHostedService
	{
		private readonly IReportServices _reportServices;
		private readonly IApprovePaymentOrder _completeApproved;

		public BackGroundServices(IServiceProvider serviceProvider)
		{
			var scoppedProvider = serviceProvider.CreateScope().ServiceProvider;
			_reportServices = scoppedProvider.GetRequiredService<IReportServices>();
			_completeApproved = scoppedProvider.GetRequiredService<IApprovePaymentOrder>();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			RecurringJob.AddOrUpdate("CheckApproved", () => _completeApproved.ApproveAsync(), Cron.Minutely());
			//RecurringJob.AddOrUpdate("SendReport", () => _reportServices.SendReportAsync(null), Cron.Monthly(28));
			//RecurringJob.TriggerJob("SendReport");

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
