using Educational_Platform.API.Authorizations;
using Educational_Platform.API.Options;
using Educational_Platform.Application.Abstractions.SecurityInterfaces;
using Educational_Platform.Application.Aggregates;
using Educational_Platform.Application.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Educational_Platform.API.Extensions
{
	public static class Registeration
	{
		public static IServiceCollection AddPresentationDependencies(
			this IServiceCollection services,
			IConfigurationManager configuration)
		{
			#region Authorizations
			services.AddAuthorization(options =>
			{
				options.AddPolicy("CourseAccess", policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new AccessCoursePolicyRequirements());
				});
				options.AddPolicy("TopicAccess", policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new AccessTopicPolicyRequirements());
				});
				options.AddPolicy("UnitAccess", policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new AccessUnitPolicyRequirements());
				});
			});
			services
				.AddScoped<IAuthorizationHandler, AccessTopicPolicyHandler>()
				.AddScoped<IAuthorizationHandler, AccessUnitPolicyHandler>()
				.AddScoped<IAuthorizationHandler, AccessCoursePolicyHandler>();

			#endregion

			#region Options
			services
				.Configure<APISecurityOptions>(configuration.GetSection("APISecurity"))
				.Configure<JwtOptions>(configuration.GetSection("Jwt"));
			#endregion

			#region AddAuthentication
			var jwt = configuration.GetSection("Jwt").Get<JwtOptions>();
			services.AddAuthentication()
				.AddJwtBearer("Bearer", cfg =>
				{
					cfg.SaveToken = true;
					cfg.TokenValidationParameters = new()
					{
						ValidateIssuer = true,
						ValidIssuer = jwt.Issuer,
						ValidateAudience = true,
						ValidAudience = jwt.Audience,
						ValidateLifetime = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
						ClockSkew = TimeSpan.Zero,
					};
				});
			#endregion

			#region RegisterServices
			services.AddScoped<IJwtTokenServices, JwtTokenServices>();
			#endregion

			return services;
		}
	}
}
