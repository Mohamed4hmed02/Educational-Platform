using Educational_Platform.Application.Abstractions.PaymentInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    public class PaymentsController(IPaymentCommandService commandService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPatch("{userEmail}")]
        public async ValueTask ConfirmPayment(string userEmail)
        {
            await commandService.ConfirmByUserEmailAsync(userEmail);
        }

        [Authorize]
        [HttpGet("{orderId}")]
        public async ValueTask<IActionResult> CreateAndGetUrl(string orderId)
        {
            return Ok(await commandService.CreateAsync(orderId));
        }
    }
}
