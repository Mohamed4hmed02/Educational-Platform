using System.Security.Claims;

namespace Educational_Platform.Application.Abstractions.SecurityInterfaces
{
    public interface IJwtTokenServices
    {
        string CreateAccessToken(ClaimsIdentity claims);
        string CreateRefreshToken();
    }
}
