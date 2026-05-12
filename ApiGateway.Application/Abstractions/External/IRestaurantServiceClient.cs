using ApiGateway.Application.DTO.RestaurantServiceClient;

namespace ApiGateway.Application.Abstractions.External
{
    public interface IRestaurantServiceClient
    {
        Task<GetRestaurantListResponseDto> GetRestaurantList(string keyword);
    }
}
