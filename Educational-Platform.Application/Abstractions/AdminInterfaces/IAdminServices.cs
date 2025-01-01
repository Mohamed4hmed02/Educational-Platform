using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;

namespace Educational_Platform.Application.Abstractions.AdminInterfaces
{
    public interface IAdminServices
    {
        ValueTask<TokensModel> AuthenticationAsync(AuthenticationRequestModel request);
        ValueTask<string> RefreshAccessTokenAsync(string refreshToken);
    }
}
