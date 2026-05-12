using System.Net.Http.Json;
using ApiGateway.Application.Abstractions.External;
using ApiGateway.Application.DTO.RestaurantServiceClient;

namespace ApiGateway.Infrastructure.Client
{
    public class RestaurantHttpClient : IRestaurantServiceClient
    {
        private readonly HttpClient _httpClient;

        public RestaurantHttpClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<GetRestaurantListResponseDto> GetRestaurantList(string keyword)
        {
            var response = await _httpClient.GetAsync($"/api/restaurant/list?keyword={Uri.EscapeDataString(keyword)}");

            if (!response.IsSuccessStatusCode)
                return new GetRestaurantListResponseDto();

            var result = await response.Content.ReadFromJsonAsync<GetRestaurantListResponseDto>();
            return result ?? new GetRestaurantListResponseDto();
        }
    }
}
