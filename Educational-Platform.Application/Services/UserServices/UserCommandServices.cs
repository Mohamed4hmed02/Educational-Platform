using FluentValidation;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.SecurityInterfaces;
using Educational_Platform.Application.Abstractions.UserInterfaces;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Educational_Platform.Application.Services.UserServices
{
	public partial class UserCommandServices(
		IUnitOfWork unitOfWork,
		IHashingServices hashing,
		IJwtTokenServices jwtToken,
		IEmailSenderServices emailSender,
		ICacheServices cacheServices,
		IValidator<User> validator,
		IConfiguration configuration,
		ILogger<UserCommandServices> log) : IUserCommandServices
	{
		public async ValueTask SendRegisterRequestAsync(string email)
		{
			if (await unitOfWork.UsersRepository.IsExistAsync(u => u.Email == email))
			{
				throw new DuplicateNameException("Invalid Email")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);
			}

			try
			{
				var code = GenerateAndCacheUserCode(email);
				if (code.IsNullOrEmpty())
					return;

				string subject = "Register New Account";
				string body = configuration["RegisterMessage"].Replace("@code", code);

				await emailSender.SendMailAsync(body, email, subject: subject);
			}
			catch
			{
				throw new InvalidDataException("Invalid Email")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
			}
		}

		public async ValueTask<object> CompleteRegisterRequestAsync(RegisterUserModel model, object code)
		{
			var cachedCode = Convert.ToString(cacheServices.GetValue(model.Email)
				?? throw new InvalidDataException("Invalid Email Or Code")
				.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest));

			if (!code.Equals(cachedCode))
				throw new InvalidDataException("Wrong Code")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

			ValidateUser(model);

			string hashedPassword = hashing.Hash(model.Password);
			User user = model;
			user.Password = hashedPassword;
			user.FullId = GenerateUserFullId();

			await unitOfWork.UsersRepository.CreateAsync(user);
			await unitOfWork.SaveChangesAsync();
			await AddCoursesUserTakesAndDeletePreviousOnesAsync(model.OldUserCourseModels, user.Id);
			cacheServices.Remove(model.Email);

			log.LogInformation("New User {@User} Registered", QueryUserModel.GetModel(user, unitOfWork));
			return user.FullId;
		}

		public async ValueTask UpdateAsync(RegisterUserModel model, object userFullId)
		{
			ValidateUser(model);

			User updatedUser = model;

			var user = await unitOfWork.UsersRepository
				.AsNoTracking()
				.ReadAsync(u => userFullId.Equals(u.FullId));

			updatedUser.FullId = user.FullId;
			updatedUser.Id = user.Id;
			updatedUser.RefreshToken = user.RefreshToken;
			updatedUser.RefreshTokenExpireDate = user.RefreshTokenExpireDate;

			if (model.IsPasswordChanged)
				updatedUser.Password = hashing.Hash(model.Password);
			else
				updatedUser.Password = user.Password;

			await AddCoursesUserTakesAndDeletePreviousOnesAsync(model.OldUserCourseModels, user.Id);
			await unitOfWork.UsersRepository.UpdateAsync(updatedUser);
			await unitOfWork.SaveChangesAsync();

			log.LogInformation("A User Was Updated To {@User}", QueryUserModel.GetModel(user, unitOfWork));
		}

		public async ValueTask DeleteAsync(object entityId)
		{
			var user = await unitOfWork.UsersRepository.DeleteAsync(u => entityId.Equals(u.FullId));
			await unitOfWork.SaveChangesAsync();

			log.LogInformation("A User {@User} Was Deleted", QueryUserModel.GetModel(user, unitOfWork));
		}

		public async ValueTask<TokensModel> AuthenticationAsync(AuthenticationRequestModel request)
		{
			User? user = default;
			try
			{
				user = await unitOfWork.UsersRepository.ReadAsync(u => u.Email == request.UserName);
			}
			catch
			{
				log.LogError("Invalid Email or Password For User UserName {UserName}", request.UserName);
				throw new NotExistException("Invalid Email Or Password");
			}

			//if (user.IsActive)
			//	throw new InvalidOperationException("User Is Aleady Authenticated Some Where")
			//		.AddToExceptionData(GlobalExceptionsDefaults.StatusCodeKey, StatusCodes.Status422UnprocessableEntity);

			if (!hashing.Verify(request.Password, user.Password))
				throw new NotExistException("Invalid Email Or Password");

			var tokens = await GetTokensAsync(user);

			user.RefreshToken = tokens.RefreshToken;
			user.RefreshTokenExpireDate = tokens.RefreshTokenExpiration;
			await unitOfWork.SaveChangesAsync();

			log.LogInformation("An User {User} Authenticated", user.Email);
			return tokens;
		}

		public async ValueTask LogOutAsync(object userFullId, object refreshToken)
		{
			var user = await unitOfWork.UsersRepository.ReadAsync(
				u => userFullId.Equals(u.FullId) && refreshToken.Equals(u.RefreshToken));

			if (!user.IsActive)
				throw new InvalidOperationException("User Is Not Authenticated")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status422UnprocessableEntity);

			await RevokeRefreshTokenAsync(user);
		}

		public async ValueTask<string> RefreshAccessTokenAsync(object userFullId, object refreshToken)
		{
			var user = await unitOfWork.UsersRepository.ReadAsync(u => userFullId.Equals(u.FullId)
			&& refreshToken.Equals(u.RefreshToken));

			if (user.IsTokenExpired)
				return "Expired Token";

			return jwtToken.CreateAccessToken(await GetClaimsAsync(user));
		}

		public async ValueTask ResetPasswordRequestAsync(string email)
		{
			if (cacheServices.GetValue(email) is null)
			{
				if (!await unitOfWork.UsersRepository.IsExistAsync(u => u.Email == email))
					throw new NotExistException("There Is No User With Such Email");

				var secretNumber = new StringBuilder();

				//Generate Secret Number
				for (int i = 0; i < 6; i++)
					secretNumber.Append(Random.Shared.Next(0, 9));

				try
				{
					await emailSender.SendMailAsync($"Here Is Your Secret Number:{secretNumber}", email, subject: "Reset Password");
					cacheServices.Cache(email, secretNumber, TimeSpan.FromMinutes(5));
				}
				catch
				{
					throw new InvalidDataException("Invalid Email")
						.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
				}
			}
		}

		public async ValueTask ResetPasswordAsync(string email, object code, string newPassword)
		{
			var cachedValue = Convert.ToString(cacheServices.GetValue(email)
				?? throw new InvalidDataException("Invalid Code Or Email")
				.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest));

			if (code.Equals(cachedValue))
			{
				var user = await unitOfWork.UsersRepository.ReadAsync(u => u.Email.Equals(email));
				user.Password = hashing.Hash(newPassword);
				await unitOfWork.SaveChangesAsync();
				cacheServices.Remove(email);

				log.LogInformation("A UserName {UserName} Password Was Changed", user.Email);
			}
			else
				throw new InvalidDataException("Wrong Or Expired Code")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
		}

		public async ValueTask ResetPasswordAsync(string email, string newPassword)
		{
			var user = await unitOfWork.UsersRepository
				.ReadAsync(u => u.Email.Equals(email));
			user.Password = hashing.Hash(newPassword);
			await unitOfWork.SaveChangesAsync();
			log.LogInformation("A UserName {UserName} Password Was Changed", user.Email);

		}
	}


	partial class UserCommandServices // Private Methods
	{
		private async ValueTask<ClaimsIdentity> GetClaimsAsync(User user)
		{
			var claims = new List<Claim>()
			{
				new(ClaimTypes.Role,"User"),
				new(ClaimTypes.NameIdentifier,user.FullId),
				new(ClaimTypes.Email,user.Email)
			};

			var courses = (await unitOfWork.UsersCoursesRepository.ReadAllAsync(uc => uc.UserId == user.Id,
				uc => new { uc.CourseId })).
			Select(uc => new Claim("Courses", uc.CourseId.ToString()));

			if (!courses.IsNullOrEmpty())
				claims.AddRange(courses);

			return new ClaimsIdentity(claims);
		}

		private async ValueTask<TokensModel> GetTokensAsync(User user)
		{
			string accessToken = jwtToken.CreateAccessToken(await GetClaimsAsync(user));
			string refreshToken = jwtToken.CreateRefreshToken();
			DateTime expiration = DateTime.UtcNow.AddDays(1);
			return new TokensModel
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				RefreshTokenExpiration = expiration
			};
		}

		private string GenerateUserFullId()
		{
			var bytes = RandomNumberGenerator.GetBytes(4);
			return Convert.ToHexString(bytes);
		}

		private async ValueTask RevokeRefreshTokenAsync(User user)
		{
			user.RefreshToken = null;
			user.RefreshTokenExpireDate = null;
			await unitOfWork.SaveChangesAsync();
		}

		private string GenerateAndCacheUserCode(string key)
		{
			var cacheCode = cacheServices.GetValue(key);
			if (cacheCode != null)
				return string.Empty;

			var code = new StringBuilder();
			for (int i = 0; i < 6; i++)
				code.Append(Random.Shared.Next(0, 9));

			cacheServices.Cache(key, code.ToString(), TimeSpan.FromMinutes(3));

			return code.ToString();
		}

		private async void ValidateUser(User user)
		{
			var result = validator.Validate(user);

			if (!result.IsValid)
				throw new InvalidDataException(result.Errors.GetAllErrorsMessages())
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

			if (await unitOfWork.UsersRepository.IsExistAsync(u => u.Email == user.Email))
				throw new InvalidDataException("Invalid Email")
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);
		}

		private async ValueTask AddCoursesUserTakesAndDeletePreviousOnesAsync(IEnumerable<OldUserCourseModel> models, int userId)
		{
			if (models is null || !models.Any())
				return;

			await unitOfWork.CoursesUserTakesRepository.DeleteWithUserIdAsync(userId);

			var cut = models.Select(c => new OldUserCourse
			{
				CourseName = c.CourseName,
				UserId = userId
			});

			await unitOfWork.CoursesUserTakesRepository.CreateAsync(cut);
			await unitOfWork.SaveChangesAsync();
		}
	}
}
