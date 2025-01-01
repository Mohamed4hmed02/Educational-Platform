using Educational_Platform.API.Options;
using Educational_Platform.Application.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Educational_Platform.API.Middlewares
{
	public class APIGatewayMiddleware(RequestDelegate next, IOptionsMonitor<APISecurityOptions> options)
	{
		private readonly string[] _keys = options.CurrentValue.APIKeys;
		public async Task Invoke(HttpContext context)
		{
			var key = context.Request.Headers["API-Key"];
			if (key.IsNullOrEmpty())
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			else if (_keys.Contains(key[0]))
				await next(context);
			else
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
		}
	}
}
