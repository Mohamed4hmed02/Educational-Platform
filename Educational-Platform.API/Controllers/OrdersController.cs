using Educational_Platform.Application.Abstractions.OrderDetails;
using Educational_Platform.Application.Abstractions.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class OrdersController(
        IOrderCommandService orderCommand,
        IOrderQueryService orderQuery,
        IOrderDetailQueryService DetailQuery) : ControllerBase
    {
        [Authorize]
        [HttpPost("{userId}")]
        public async ValueTask PlaceOrder(string userId)
        {
            await orderCommand.Make(userId);
        }

        [Authorize]
        [HttpDelete("{orderId}")]
        public async ValueTask CancelOrder(string orderId)
        {
            await orderCommand.Cancel(orderId);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async ValueTask<IActionResult> GetUserOrdersInformation(string userId)
        {
            return Ok(await orderQuery.GetModels(userId));
        }

        [Authorize]
        [HttpGet("Details/{orderId}")]
        public async ValueTask<IActionResult> GetOrderDetailsInformation(string orderId)
        {
            return Ok(await DetailQuery.GetModels(orderId));
        }
    }
}
