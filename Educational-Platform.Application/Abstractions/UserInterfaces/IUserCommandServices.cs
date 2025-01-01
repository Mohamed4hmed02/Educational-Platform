using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.UserInterfaces
{
	public interface IUserCommandServices : IDeleteable<User>
	{
		ValueTask<string> RefreshAccessTokenAsync(object userFullId, object refreshToken);
		ValueTask SendRegisterRequestAsync(string email);
		ValueTask<object> CompleteRegisterRequestAsync(RegisterUserModel model, object code);
		ValueTask UpdateAsync(RegisterUserModel model, object userFullId);
		ValueTask<TokensModel> AuthenticationAsync(AuthenticationRequestModel request);

		/// <summary>
		/// Sending Seccret Key With 6 Numbers To Specific email parameter
		/// </summary>
		/// <param name="email"></param>
		/// <returns>No Return</returns>
		/// <exception cref="InvalidDataException"></exception>
		ValueTask ResetPasswordRequestAsync(string email);
		ValueTask ResetPasswordAsync(string email, object code, string newPassword);
		ValueTask ResetPasswordAsync(string email, string newPassword);
		ValueTask LogOutAsync(object userFullId, object refreshToken);
	}
}
