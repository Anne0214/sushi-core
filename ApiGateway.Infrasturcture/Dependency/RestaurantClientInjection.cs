using ApiGateway.Application.Abstractions.External;
using ApiGateway.Infrastructure.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.Infrastructure.Dependency
{
    public static class RestaurantClientInjection
    {
        public static IServiceCollection AddRestaurantClient(this IServiceCollection services, IConfiguration config)
        {
            var baseUri = new Uri(config.GetValue<string>("Restaurant:Domain")!);
            services.AddHttpClient<IRestaurantServiceClient, RestaurantHttpClient>(client =>
            {
                client.BaseAddress = baseUri;
                client.Timeout = TimeSpan.FromSeconds(config.GetValue<int>("Restaurant:ConnectTimeout"));
            });

            return services;
        }
    }
}
