using System.Threading.RateLimiting;
using Educational_Platform.API.Extensions;
using Educational_Platform.API.Middlewares;
using Educational_Platform.Application;
using Educational_Platform.Infrastructure;
using Hangfire;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("apisettings.json");

        builder.Environment.EnvironmentName = builder.Configuration["ENVIRONMENT"] ?? "Development";

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("apisettings.development.json");
            builder.Services.AddSwaggerGen();
        }

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,         // Number of requests allowed
                Window = TimeSpan.FromMinutes(1)  // Time window
            }));
        });

        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("Any", bld =>
            {
                bld.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });

            opt.AddPolicy("Production", bld =>
            {
                bld.WithOrigins("https://evidencebased-fitness.com").AllowAnyHeader().AllowAnyMethod();
            });
        });

        // Add Layers Dependencies
        builder.Services
            .AddPresentationDependencies(builder.Configuration)
            .AddInfrastructureDependencies(builder.Configuration)
            .AddApplicationDependencies(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsStaging())
            app.UseCors("Any");
        else if (app.Environment.IsProduction())
            app.UseCors("Any");

        app.UseSerilogRequestLogging();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHangfireDashboard();
        }
        else
        {
            app.UseMiddleware<APIGatewayMiddleware>();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseRateLimiter();

        app.MapControllers();

        app.Run();
    }
}
