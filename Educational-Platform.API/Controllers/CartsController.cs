using Educational_Platform.Application.Abstractions.CartDetailInterfaces;
using Educational_Platform.Application.Abstractions.CartInterfaces;
using Educational_Platform.Application.Models.CommandModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    public class CartsController(
        ICartCommandService cartServices,
        ICartQueryService cartQueryServices,
        ICartDetailCommandService detailsServices,
        ICartDetailQueryService cartDetailQueryServices) : ControllerBase
    {

        [Authorize]
        [HttpPost("{userId}")]
        public async ValueTask<IActionResult> MakeCart(string userId)
        {
            return Ok(await cartServices.AddAsync(userId));
        }

        [Authorize]
        [HttpDelete("{cartId}")]
        public async ValueTask EmptyCart(string cartId)
        {
            await cartServices.EmptyCartAsync(cartId);
        }

        [Authorize]
        [HttpPatch("Details")]
        public async ValueTask AddCartDetail(CommandCartDetailModel model)
        {
            await detailsServices.AddAsync(model);
        }

        [Authorize]
        [HttpDelete("Details")]
        public async ValueTask RemoveCartDetail(CommandCartDetailModel model)
        {
            await detailsServices.RemoveAsync(model);
        }

        [Authorize]
        [HttpGet("{cartId}/Details")]
        public async ValueTask<IActionResult> GetCartDetails(string cartId)
        {
            return Ok(await cartDetailQueryServices.GetActiveCartDetailsAsync(cartId));
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async ValueTask<IActionResult> GetUserCart(string userId)
        {
            return Ok(await cartQueryServices.GetCartAsync(userId));
        }
    }
}
