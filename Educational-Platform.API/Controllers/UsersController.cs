using Educational_Platform.Application.Abstractions.UserInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
    [Authorize]
	[Route("api/v1/[controller]/")]
	[ApiController]
	public class UsersController(
		IUserCommandServices userCommandServices,
		IUserQueryServices userQueryServices,
		IEmailSenderServices emailSender) : ControllerBase
	{
		[AllowAnonymous]
		[HttpHead("{email}")]
		public async ValueTask RegisterRequest(string email)
		{
			await userCommandServices.SendRegisterRequestAsync(email);
		}

		[AllowAnonymous]
		[HttpPost("{code}")]
		public async ValueTask CompleteRegisterRequest(RegisterUserModel model, string code)
		{
			await userCommandServices.CompleteRegisterRequestAsync(model, code);
		}

		[AllowAnonymous]
		[HttpPost("Messaging")]
		public async ValueTask SendMessageToAdmin(MessageModel model)
		{
			await emailSender.AcceptMailAsync(model.Content, model.From, subject: model.Subject);
		}

		[AllowAnonymous]
		[HttpPatch]
		public async ValueTask<TokensModel> Authenticate(AuthenticationRequestModel request)
		{
			return await userCommandServices.AuthenticationAsync(request);
		}

		[HttpPatch("{userId}/{refreshToken}")]
		public async ValueTask LogOut(string userId, string refreshToken)
		{
			await userCommandServices.LogOutAsync(userId, refreshToken);
		}

		[AllowAnonymous]
		[HttpGet("{userId}/{refreshToken}")]
		public async ValueTask<IActionResult> RefreshToken(string userId, string refreshToken)
		{
			return Ok(await userCommandServices.RefreshAccessTokenAsync(userId, refreshToken));
		}

		[HttpDelete("{userId}")]
		public async ValueTask DeleteUser(string userId)
		{
			await userCommandServices.DeleteAsync(userId);
		}

		[HttpPut]
		public async ValueTask UpdateUser(RegisterUserModel model, string userId)
		{
			await userCommandServices.UpdateAsync(model, userId);
		}

		[HttpGet("{userId}")]
		public async ValueTask<IActionResult> GetUser(string userId)
		{
			return Ok(await userQueryServices.GetUserAsync(userId));
		}

		[AllowAnonymous]
		[HttpHead("{email}/Reset")]
		public async ValueTask ResetPasswordRequest(string email)
		{
			await userCommandServices.ResetPasswordRequestAsync(email);
		}

		[AllowAnonymous]
		[HttpPatch("{email}/Reset/{code}/{newPassword}")]
		public async ValueTask ResetPassword(string email, string code, string newPassword)
		{
			await userCommandServices.ResetPasswordAsync(email, code, newPassword);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{email}/Reset/{newPassword}")]
		public async ValueTask ResetPasswordByAdmin(string email, string newPassword)
		{
			await userCommandServices.ResetPasswordAsync(email, newPassword);
		}
	}
}
