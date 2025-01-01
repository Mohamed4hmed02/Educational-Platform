using System.Security.Claims;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.UserCourseInterfaces;
using Educational_Platform.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Educational_Platform.API.Authorizations
{
    public class AccessCoursePolicyHandler(
        IUnitOfWork unitOfWork,
        IUserCourseQueryServices userCourse) : AuthorizationHandler<AccessCoursePolicyRequirements>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessCoursePolicyRequirements requirement)
        {
            var httpContext = context.Resource as HttpContext;

            if (IsUserAdmin(httpContext))
            {
                context.Succeed(requirement);
                return;
            }

            var courseId = GetCourseId(httpContext);

            if (courseId == 0)
            {
                context.Fail();
                return;
            }

            bool canAccess = await CanUserAccessCourse(httpContext, courseId);

            if (!canAccess)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }

        private int GetCourseId(HttpContext? context)
        {
            var routeValues = context?.Request.RouteValues;
            object? courseId = default;

            routeValues?.TryGetValue("courseId", out courseId);
            return Convert.ToInt32(courseId);
        }

        private async ValueTask<bool> CanUserAccessCourse(HttpContext? context, int courseId)
        {
            var userFullId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userFullId.IsNullOrEmpty())
                return false;

            var userId = await unitOfWork.UsersRepository.ReadAsync(u => u.FullId == userFullId, u => u.Id);

            return await userCourse.CanUserAccessCourseAsync(userId, courseId);
        }

        private bool IsUserAdmin(HttpContext? context)
        {
            return context?.User.FindFirstValue(ClaimTypes.Role)
                == "Admin" ? true : false;
        }
    }

    public class AccessCoursePolicyRequirements : IAuthorizationRequirement
    {
    }
}
