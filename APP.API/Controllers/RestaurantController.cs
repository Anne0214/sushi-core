using APP.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sushi.All.Infrastructure.Web.Attributes;

namespace APP.API.Controllers
{
    public class RestaurantController : Controller
    {
        [Authorize]
        [ServiceFilter(typeof(TokenBucketFilter))]
        [RouteId("Restaurant/List")]
        public IActionResult List()
        {
            return Content("OK");
        }
    }
}
