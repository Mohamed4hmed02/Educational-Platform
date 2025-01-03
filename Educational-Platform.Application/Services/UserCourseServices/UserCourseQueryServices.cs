using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.UserCourseInterfaces;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;

namespace Educational_Platform.Application.Services.UserCourseServices
{
    public class UserCourseQueryServices(
		IUnitOfWork unitOfWork,
		IEmailSenderServices emailSender,
		ILogger<UserCourseQueryServices> logger) : IUserCourseQueryServices
	{
		private readonly static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		public async ValueTask<bool> CanUserAccessCourseAsync(object userId, object courseId)
		{
			return await unitOfWork.UsersCoursesRepository.IsExistAsync(uc => uc.UserId.Equals(userId)
			&& uc.CourseId.Equals(courseId));
		}

		public async ValueTask GetCertificateAsync(object userFullId, object courseId)
		{
			var user = await unitOfWork.UsersRepository
				.AsNoTracking()
				.ReadAsync(u => u.FullId.Equals(userFullId));

			var userCourse = await unitOfWork.UsersCoursesRepository
				.AsNoTracking()
				.ReadAsync(uc => uc.UserId == user.Id && uc.CourseId.Equals(courseId));

			if (!userCourse.PassedCourse)
				throw new InvalidOperationException("User didn't pass this course")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status403Forbidden);

			semaphore.Wait(TimeSpan.FromSeconds(5));
			try
			{
				using (var stream = new MemoryStream())
				{
					PrepareCertificate(user.FirstName + " " + user.LastName, userCourse.PassedCourseDate, stream);
					var certificate = new Attachment(stream, "Certificate.jpg");
					await emailSender.SendMailAsync("Congratulations On Passing Our Course!", user.Email, subject: "Certificate", attachment: certificate);
					stream.Dispose();
				}
			}
			catch (Exception ex)
			{
				logger.LogError("Error During Send Certificate,{error}", ex.Message);
				semaphore.Release();
				throw;
			}
			semaphore.Release();
		}

		public async ValueTask<int> GetMaxUnitUserCanAccess(object userFullId, object courseId)
		{
			var userId = await unitOfWork.UsersRepository
				.AsNoTracking()
				.ReadAsync(
				u => u.FullId.Equals(userFullId),
				u => u.Id);

			return await unitOfWork.UsersCoursesRepository
				.AsNoTracking()
				.ReadAsync(
				uc => uc.CourseId.Equals(courseId) &&
				uc.UserId == userId,
				uc => uc.CurrentUnitId);
		}

		public async ValueTask<IEnumerable<QueryUserCourseModel>> GetUserCoursesAsync(object userFullId)
		{
			var user = await unitOfWork.UsersRepository
				.AsNoTracking()
				.ReadAsync(u => userFullId.Equals(u.FullId),
					u => new
					{
						u.Id,
						u.FullId
					}
				);

			return await unitOfWork.UsersCoursesRepository
				.AsNoTracking()
				.ReadAllAsync(c => c.UserId == user.Id,
					x => new QueryUserCourseModel
					{
						UserFullId = user.FullId,
						CourseName = x.Course.Name,
						CourseId = x.CourseId,
						StartDate = x.StartDate,
						MaxUnitUserCanAccess = x.CurrentUnitId
					}, [c => c.Course]
				);
		}

		private void PrepareCertificate(string name, DateTime dateTime, Stream stream)
		{
			Bitmap image = new Bitmap(@"wwwroot\Certificate.jpg");
			Graphics g = Graphics.FromImage(image);
			Font nameFont = new Font("Arial", 40);
			Font dateFont = new Font("Arial", 13);

			SolidBrush brush = new SolidBrush(Color.Black);

			// Set the text alignment to center
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			// Get the width and height of the image
			int imageWidth = image.Width;
			int imageHeight = image.Height;

			// Draw the centered text on the image
			g.DrawString(name.ToUpper(), nameFont, brush, new RectangleF(0, 0, imageWidth, imageHeight), format);
			g.DrawString($"AT {DateOnly.FromDateTime(dateTime).ToLongDateString()}", dateFont, brush, 1020, 558);

			// Save the modified Bitmap to a new PNG file
			image.Save(stream, ImageFormat.Png);
		}
	}
}
