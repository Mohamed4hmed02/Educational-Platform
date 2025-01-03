using System.Security.Claims;
using Educational_Platform.Application.Abstractions.AdminInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.SecurityInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services
{
    public class AdminServices(
        IUnitOfWork unitOfWork,
        IJwtTokenServices jwtToken,
        IHashingServices hashing,
        ILogger<AdminServices> log) : IAdminServices
    {
        public async ValueTask<TokensModel> AuthenticationAsync(AuthenticationRequestModel request)
        {
            try
            {
                if (!unitOfWork.AdminsRepository.IsExist
                    (a => a.UserName == request.UserName,
                    a => a.UserName,
                    request.UserName,
                    StringComparison.Ordinal,
                    out var admin))
                    throw new NotExistException();

                if (!hashing.Verify(request.Password, admin.Password))
                    throw new NotExistException();

                var claims = new ClaimsIdentity(
                [
                    new(ClaimTypes.Role,"Admin")
                ]);

                var accessToken = jwtToken.CreateAccessToken(claims);
                var refreshToken = jwtToken.CreateRefreshToken();
                admin.RefreshToken = refreshToken;
                var expireDate = DateTime.UtcNow.AddDays(1);
                admin.RefreshTokenExpireDate = expireDate;

                await unitOfWork.SaveChangesAsync();
                log.LogInformation("An Admin Authenticated {@Admin}", admin);

                return new TokensModel
                {
                    RefreshToken = refreshToken,
                    AccessToken = accessToken,
                    RefreshTokenExpiration = expireDate
                };
            }
            catch
            {
                throw new NotExistException("Invalid Email Or Password");
            }
        }

        public async ValueTask<string> RefreshAccessTokenAsync(string refreshToken)
        {
            try
            {
                var admin = await unitOfWork.AdminsRepository.ReadAsync(a => a.RefreshToken == refreshToken);

                if (admin.IsTokenExpired)
                    return "Expired Token";

                var claims = new ClaimsIdentity(
                [
                    new(ClaimTypes.Role,"Admin")
                ]);

                return jwtToken.CreateAccessToken(claims);
            }
            catch
            {
                throw new InvalidDataException("Wrong Or Expired Token")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
            }
        }
    }
}
