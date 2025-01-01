using Educational_Platform.Application.Abstractions.SecurityInterfaces;
using Educational_Platform.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Educational_Platform.Application.Aggregates
{
    public class JwtTokenServices(IOptionsMonitor<JwtOptions> options) : IJwtTokenServices
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
        public string CreateAccessToken(ClaimsIdentity claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = options.CurrentValue.Issuer,
                Audience = options.CurrentValue.Audience,
                Expires = DateTime.UtcNow.AddMinutes(options.CurrentValue.LifeTime),
                Subject = claims,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.CurrentValue.SigningKey)), SecurityAlgorithms.HmacSha256)
            };
            var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return _jwtSecurityTokenHandler.WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var number = RandomNumberGenerator.GetBytes(32);
            return Base64UrlEncoder.Encode(number);
        }
    }
}
