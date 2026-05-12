using ApiGateway.Application.UseCase;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.Abstractions
{
    public interface IGetRestaurantListUseCase
    {
        Task<Result<RestaurantListRes>> ExecuteAsync(string keyword);
    }
}
