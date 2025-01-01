using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.TopicInterfaces;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Educational_Platform.API.Authorizations
{
	public class AccessTopicPolicyHandler(
		 IUnitOfWork unitOfWork,
		 ITopicQueryServices topicServices,
		 IUnitQueryServices unitServices) : AuthorizationHandler<AccessTopicPolicyRequirements>
	{
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessTopicPolicyRequirements requirement)
		{
			var httpContext = context.Resource as HttpContext;

			if (IsUserAdmin(httpContext))
			{
				context.Succeed(requirement);
				return;
			}

			int topicId = GetTopicIdValue(httpContext);

			if (topicId == 0)
			{
				context.Fail();
				return;
			}

			bool CanAccess = await CanUserAccessTopic(httpContext, topicId);

			if (!CanAccess)
			{
				context.Fail();
				return;
			}

			context.Succeed(requirement);
		}

		private int GetTopicIdValue(HttpContext? context)
		{
			var routeValues = context?.Request.RouteValues;
			object? unitId = default;

			routeValues?.TryGetValue("topicId", out unitId);
			return Convert.ToInt32(unitId);
		}

		private async ValueTask<bool> CanUserAccessTopic(HttpContext? context, int topicId)
		{
			var userFullId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userFullId.IsNullOrEmpty())
				return false;

			var userId = await unitOfWork.UsersRepository
				.AsNoTracking()
				.ReadAsync(
				u => u.FullId == userFullId,
				u => u.Id);

			var unitId = Convert.ToInt32(await topicServices.BelongToUnitAsync(topicId));

			return await unitServices.CanUserAccessUnitAsync(unitId, userId);
		}

		private bool IsUserAdmin(HttpContext? context)
		{
			return context?.User.FindFirstValue(ClaimTypes.Role)
				== "Admin";
		}
	}

	public class AccessTopicPolicyRequirements : IAuthorizationRequirement
	{

	}
}
