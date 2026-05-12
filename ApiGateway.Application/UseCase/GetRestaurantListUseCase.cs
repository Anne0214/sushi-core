using ApiGateway.Application.Abstractions.External;
using ApiGateway.Application.DTO.RestaurantServiceClient;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.UseCase
{
    public record RestaurantItemRes(long RestaurantId, string Name);
    public record RestaurantListRes(List<RestaurantItemRes> RestaurantList);

    public class GetRestaurantListUseCase : Abstractions.IGetRestaurantListUseCase
    {
        private readonly IRestaurantServiceClient _restaurantService;

        public GetRestaurantListUseCase(IRestaurantServiceClient restaurantService)
            => _restaurantService = restaurantService;

        public async Task<Result<RestaurantListRes>> ExecuteAsync(string keyword)
        {
            try
            {
                var response = await _restaurantService.GetRestaurantList(keyword);

                var items = response.RestaurantList
                    .Select(r => new RestaurantItemRes(r.RestaurantId, r.Name))
                    .ToList();

                return Result<RestaurantListRes>.Ok(new RestaurantListRes(items));
            }
            catch (Exception ex)
            {
                return Result<RestaurantListRes>.Fail(
                    new Error("GetRestaurantListUseCase.Exception", ex.Message, ErrorType.Unexpected));
            }
        }
    }
}
