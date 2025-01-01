using FluentValidation;
using Educational_Platform.Application.Abstractions.AdminInterfaces;
using Educational_Platform.Application.Abstractions.BookInterfaces;
using Educational_Platform.Application.Abstractions.BookServices;
using Educational_Platform.Application.Abstractions.CartDetailInterfaces;
using Educational_Platform.Application.Abstractions.CartInterfaces;
using Educational_Platform.Application.Abstractions.CourseInterfaces;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Abstractions.OrderDetails;
using Educational_Platform.Application.Abstractions.Orders;
using Educational_Platform.Application.Abstractions.PaymentInterfaces;
using Educational_Platform.Application.Abstractions.Quizzes;
using Educational_Platform.Application.Abstractions.SecurityInterfaces;
using Educational_Platform.Application.Abstractions.TopicInterfaces;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Abstractions.UserCourseInterfaces;
using Educational_Platform.Application.Abstractions.UserInterfaces;
using Educational_Platform.Application.Aggregates;
using Educational_Platform.Application.Aggregates.CachingItemsService;
using Educational_Platform.Application.Options;
using Educational_Platform.Application.Services;
using Educational_Platform.Application.Services.BookServices;
using Educational_Platform.Application.Services.CartDetailService;
using Educational_Platform.Application.Services.CartService;
using Educational_Platform.Application.Services.CourseServices;
using Educational_Platform.Application.Services.OrderDetailServices;
using Educational_Platform.Application.Services.OrderServices;
using Educational_Platform.Application.Services.PaymentDetailServices;
using Educational_Platform.Application.Services.PaymentService;
using Educational_Platform.Application.Services.QuizServices;
using Educational_Platform.Application.Services.TopicServices;
using Educational_Platform.Application.Services.UnitServices;
using Educational_Platform.Application.Services.UserCourseServices;
using Educational_Platform.Application.Services.UserServices;
using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Implementaions.Validators;
using Educational_Platform.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Educational_Platform.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(
            this IServiceCollection services,
            IConfigurationManager configuration,
            IWebHostEnvironment environment)
        {
            #region AddJsonFiles
            configuration.AddJsonFile("Settings\\applicationsettings.json");
            if (environment.IsDevelopment())
                configuration.AddJsonFile("Settings\\applicationsettings.development.json");
            else if (environment.IsStaging())
                configuration.AddJsonFile("Settings\\applicationsettings.staging.json");
            #endregion

            #region Options		
            services
                .Configure<AppSecurityOptions>(configuration.GetSection("AppSecurity"))
                .Configure<PaymentRequestOptions>(configuration.GetSection("PaymentRequest"))
                .Configure<PaginationOptions>(configuration.GetSection("Pagination"))
                .Configure<CurrencyOptions>(configuration.GetSection("Currency"));
            #endregion

            #region RegisterServices
            services
                .AddScoped<IAdminServices, AdminServices>()
                .AddScoped<IHashingServices, HashingServices>()
                .AddScoped<ICourseCommandServices, CourseCommandServices>()
                .AddScoped<ITopicCommandServices, TopicCommandServices>()
                .AddScoped<IBookCommandServices, BookCommandServices>()
                .AddScoped<IUserCommandServices, UserCommandServices>()
                .AddScoped<IUserCourseCommandServices, UserCourseCommandServices>()
                .AddScoped<ICartCommandService, CartCommandService>()
                .AddScoped<IPaymentCommandService, PaymentCommandSrvice>()
                .AddScoped<ICartDetailCommandService, CartDetailCommandService>()
                .AddScoped<CachingItemStrategyFactory>()
                .AddScoped<ICachingItemService, CachingItemService>()
                .AddScoped<IUnitCommandServices, UnitCommandServices>()
                .AddScoped<IQuizCommandService, QuizzesCommandService>()
                .AddScoped<IQuizQueryService, QuizzesQueryService>()
                .AddScoped<IOrderCommandService, OrderCommandService>()
                .AddScoped<IOrderQueryService, OrderQueryService>()
                .AddScoped<IOrderDetailCommandService, OrderDetailCommandService>()
                .AddScoped<IOrderDetailQueryService, OrderDetailQueryService>();
            #endregion

            #region RegisterQueryServices
            services.AddScoped<ICartQueryService, CartQueryService>()
                    .AddScoped<ICartDetailQueryService, CartDetailQueryService>()
                    .AddScoped<IUserCourseQueryServices, UserCourseQueryServices>()
                    .AddScoped<IUserQueryServices, UserQueryServices>()
                    .AddScoped<IBookQueryServices, BookQueryServices>()
                    .AddScoped<ICourseQueryServices, CourseQueryServices>()
                    .AddScoped<ITopicQueryServices, TopicQueryServices>()
                    .AddScoped<IUnitQueryServices, UnitQueryServices>();
            #endregion

            #region Validators
            services
                .AddScoped<IValidator<User>, UserValidator>()
                .AddScoped<IValidator<Book>, BookValidator>()
                .AddScoped<IValidator<Course>, CourseValidator>()
                .AddScoped<IEntityValidator<CartDetail>, CartDetailValidator>();
            #endregion

            return services;
        }
    }
}
