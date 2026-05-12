using Restaurant.API.Models.Dto;

namespace Restaurant.API.Controllers
{
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private static readonly List<RestaurantItem> _restaurants = new()
        {
            new(1, "壽司太郎"),
            new(2, "壽司花子"),
            new(3, "拉麵一番"),
            new(4, "天婦羅本舖"),
            new(5, "燒肉王"),
        };

        [HttpGet("api/restaurant/list")]
        public IActionResult List([FromQuery] string keyword = "")
        {
            var list = string.IsNullOrWhiteSpace(keyword)
                ? _restaurants
                : _restaurants.Where(r => r.Name.Contains(keyword)).ToList();

            return Ok(new RestaurantListResponse(list));
        }
    }
}
