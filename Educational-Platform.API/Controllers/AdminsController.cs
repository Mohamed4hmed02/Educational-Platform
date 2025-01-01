using Educational_Platform.Application.Abstractions.AdminInterfaces;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
    [Route("api/v1/[controller]/")]
	[ApiController]
	public class AdminsController(IAdminServices adminServices,IReportServices reportServices) : ControllerBase
	{
		[HttpPatch]
		public async ValueTask<IActionResult> Authentication(AuthenticationRequestModel request)
		{
			return Ok(await adminServices.AuthenticationAsync(request));
		}

		[HttpGet("{refreshToken}")]
		public async ValueTask<IActionResult> RefreshToken(string refreshToken)
		{
			return Ok(await adminServices.RefreshAccessTokenAsync(refreshToken));
		}

		[HttpHead("Reports")]
		public async ValueTask SendReportAsync(string? To)
		{
			await reportServices.SendReportAsync(To);
		}
	}
}
