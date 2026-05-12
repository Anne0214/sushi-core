namespace Restaurant.API.Models.Dto
{
    public record RestaurantItem(long RestaurantId, string Name);
    public record RestaurantListResponse(List<RestaurantItem> RestaurantList);
}
