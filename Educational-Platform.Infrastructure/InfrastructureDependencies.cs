using Hangfire;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.PaymentGatewayServices;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Infrastructure.Aggregates;
using Educational_Platform.Infrastructure.Implementations.Caches;
using Educational_Platform.Infrastructure.Implementations.Context;
using Educational_Platform.Infrastructure.Implementations.EmailsSender;
using Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal;
using Educational_Platform.Infrastructure.Implementations.Reports;
using Educational_Platform.Infrastructure.Implementations.Repos;
using Educational_Platform.Infrastructure.Implementations.Repos.Special;
using Educational_Platform.Infrastructure.Implementations.Storages;
using Educational_Platform.Infrastructure.Implementations.VideoHostings;
using Educational_Platform.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Educational_Platform.Infrastructure
{
	public static class InfrastructureDependencies
	{
		public static IServiceCollection AddInfrastructureDependencies(
			this IServiceCollection services,
			IConfigurationManager configuration)
		{
			#region DataBase
			services.AddDbContext<AppDbContext>(op =>
			{
				op.UseSqlServer(configuration.GetConnectionString("sqlconnection"));
			});
			#endregion

			#region Options
			services
				.Configure<VimeoOptions>(configuration.GetSection("Vimeo"))
				.Configure<SMTPOptions>(configuration.GetSection("smtpClientCredentials"))
				.Configure<GoogleDriveOptions>(configuration.GetSection("DriveOptions"))
				.Configure<HostStorageOptions>(configuration.GetSection("HostStorage"))
				.Configure<GoogleSpreadSeetOptions>(configuration.GetSection("SpreadSheetOptions"))
				.Configure<PaypalOptions>(configuration.GetSection("Paypal"));
            #endregion

            #region HangFire
            services.AddHangfire(cfg =>
			{
				cfg.UseSqlServerStorage(configuration.GetConnectionString("HangFire"));
			}).AddHangfireServer();

			services.AddHostedService<BackGroundServices>();

			#endregion

			#region RegisterServices
			services
				.AddScoped<IVideoHostServices, VimeoVideoHostService>()
				.AddScoped<IReportServices, ReportServices>()
				.AddSingleton<IStorageService, HostStorageServices>()
				.AddScoped<IEmailSenderServices, SmtpClientEmailSenderServices>()
				.AddScoped(typeof(IRepositoryBase<>), typeof(AsyncRepository<>))
				.AddScoped<IUnitOfWork, UnitOfWork>()
				.AddScoped<PaymentGatewayAbstractService, PaypalPaymentGatewayService>()
				.AddScoped<PaypalPaymentGatewayServiceBase>()
				.AddScoped<ICartAsyncRepository, CartAsyncRepository>()
				.AddScoped<ICartDetailAsyncRepository, CartDetailAsyncRepository>()
				.AddScoped<ITopicAsyncRepository, TopicAsyncRepository>()
				.AddScoped<IUnitAsyncRepository, UnitAsyncRepository>()
				.AddScoped<IUserCourseAsyncRepository, UserCourseAsyncRepository>()
				.AddScoped<IBookAsyncRepository, BookAsyncRepository>()
				.AddScoped<IOldUserCoursesAsyncRepository, OldUserCoursesAsyncRepository>()
				.AddScoped<IApprovePaymentOrder, CompleteApproved>()
				.AddSingleton<ICacheServices, MemoryCacheServices>(service =>
				{
					IOptions<MemoryCacheOptions> memoryOptions =
					new MemoryCacheOptions
					{

					};
					var memoryCache = new MemoryCache(memoryOptions);
					return new MemoryCacheServices(memoryCache);
				});
			#endregion

			return services;
		}
	}
}
