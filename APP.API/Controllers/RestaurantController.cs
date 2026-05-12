using ApiGateway.Application.Abstractions;
using APP.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sushi.All.Infrastructure.Web.Attributes;

namespace APP.API.Controllers
{
    public class RestaurantController(IGetRestaurantListUseCase getRestaurantListUseCase) : Controller
    {
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(TokenBucketFilter))]
        [RouteId("Restaurant/List")]
        public async Task<IActionResult> List([FromQuery] string keyword = "")
        {
            var result = await getRestaurantListUseCase.ExecuteAsync(keyword);

            if (!result.IsSuccess)
                return BadRequest(result.Error?.ToString() ?? "Failed to get restaurant list.");

            return Ok(result.Value);
        }
    }
}
