using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Abstractions.UserCourseInterfaces;
using Educational_Platform.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Educational_Platform.API.Authorizations
{
	public class AccessUnitPolicyHandler(
		IUnitOfWork unitOfWork,
		IUnitQueryServices unitQueryServices) : AuthorizationHandler<AccessUnitPolicyRequirements>
	{
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessUnitPolicyRequirements requirement)
		{
			var httpContext = context.Resource as HttpContext;

			if (IsUserAdmin(httpContext))
			{
				context.Succeed(requirement);
				return;
			}

			int unitId = GetUnitIdValue(httpContext);

			if (unitId == 0)
			{
				context.Fail();
				return;
			}

			bool CanAccess = await CanUserAccessUnitAsync(httpContext, unitId);

			if (!CanAccess)
			{
				context.Fail();
				return;
			}

			context.Succeed(requirement);
		}

		private int GetUnitIdValue(HttpContext? context)
		{
			var routeValues = context?.Request.RouteValues;
			object? unitId = default;

			routeValues?.TryGetValue("unitId", out unitId);
			return Convert.ToInt32(unitId);
		}

		private async ValueTask<bool> CanUserAccessUnitAsync(HttpContext? context, int unitId)
		{
			var userFullId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userFullId.IsNullOrEmpty())
				return false;

			var userId = await unitOfWork.UsersRepository.ReadAsync(u => u.FullId == userFullId,
				u => u.Id);

			return await unitQueryServices.CanUserAccessUnitAsync(unitId, userId);
		}

		private bool IsUserAdmin(HttpContext? context)
		{
			return context?.User.FindFirstValue(ClaimTypes.Role)
				== "Admin" ? true : false;
		}
	}
	public class AccessUnitPolicyRequirements : IAuthorizationRequirement
	{

	}
}
