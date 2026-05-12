namespace ApiGateway.Application.DTO.RestaurantServiceClient
{
    public class RestaurantItemDto
    {
        public long RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GetRestaurantListResponseDto
    {
        public List<RestaurantItemDto> RestaurantList { get; set; } = new();
    }
}
