using Educational_Platform.Application.Abstractions.CartDetailInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.UserCourseInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Educational_Platform.Application.Services.UserCourseServices
{
    public class UserCourseCommandServices(
		IUnitOfWork unitOfWork,
		ICartDetailCommandService detailCommandServices,
		ILogger<UserCourseCommandServices> log) : IUserCourseCommandServices
	{
		public async ValueTask AddCourseToUserAsync(CommandUserCourseModel model)
		{
			var user = await CheckForAddingCourseToUser(model);

			var firstUnitId = 1;

			await unitOfWork.UsersCoursesRepository.CreateAsync(new UserCourse
			{
				CourseId = model.CourseId,
				UserId = user.Id,
				StartDate = DateTime.UtcNow,
				CurrentUnitId = Convert.ToInt32(firstUnitId)
			});

			await DeleteCourseFromCartIfExistAsync(user, model.CourseId);

			await unitOfWork.SaveChangesAsync();

			var course = await unitOfWork.CoursesRepository.ReadAsync(model.CourseId);

			log.LogInformation("An User WithId {UserId} Has Given Access To Course WithId {UnitId}", user.FullId, course.Id);
		}

		private async ValueTask<User> CheckForAddingCourseToUser(CommandUserCourseModel model)
		{
			if (!await unitOfWork.CoursesRepository.IsExistAsync(c => c.Id == model.CourseId))
				throw new NotExistException("Course Is Not Exist");

			var user = await unitOfWork.UsersRepository
				.ReadAsync(u => u.FullId == model.UserFullIdOrEmail || u.Email == model.UserFullIdOrEmail);

			if (await unitOfWork.UsersCoursesRepository.IsExistAsync(a => a.UserId == user.Id && a.CourseId == model.CourseId))
				throw new DuplicateNameException("This User Already Has Access To The Course")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);

			return user;
		}

		private async Task DeleteCourseFromCartIfExistAsync(User user, int courseId)
		{
			CartDetail? detail = default;

			try
			{
				var payment = await unitOfWork.CartsRepository
					.AsNoTracking()
					.ReadAsync(c => c.UserId == user.Id);

				detail = await unitOfWork.CartDetailsRepository
					.ReadAsync(d =>
					d.CartId == payment.Id &&
					d.ProductId == courseId &&
					d.ProductType == Domain.Enums.ProductTypes.Course);

				await detailCommandServices.RemoveAsync(CommandCartDetailModel.GetCartDetailModel(detail));
			}
			catch (Exception ex)
			{
				log.LogError("Error during removing course with Id {} from cart,{errorMessage}", courseId, ex.Message);
				return;
			}
		}
	}
}
