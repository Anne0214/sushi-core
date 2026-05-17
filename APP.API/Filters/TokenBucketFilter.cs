using ApiGateway.Application.Abstractions;
using ApiGateway.Domain.RateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sushi.All.Infrastructure.Web.Attributes;

namespace APP.API.Filters
{
    public class TokenBucketFilter : IAsyncActionFilter
    {
        private readonly IRateLimiter _rateLimiter;
        private readonly IConfiguration _config;

        public TokenBucketFilter(IRateLimiter rateLimiter, IConfiguration config)
        {
            _rateLimiter = rateLimiter;
            _config = config;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 讀取用戶識別

            var userIdStr = context.HttpContext.User.FindFirst("userId")?.Value??"";

            if (!long.TryParse(userIdStr, out long userId))
            {
                context.Result = new ContentResult
                {
                    Content = "Unauthorized",
                    StatusCode = 401
                };
                return;
            }

            var policy = new RateLimitPolicy(
                int.Parse(_config.GetValue<string>("RateLimitPolicy:Capacity")),
                int.Parse( _config.GetValue<string>("RateLimitPolicy:RefillTokens")),
                int.Parse(_config.GetValue<string>("RateLimitPolicy:RefillIntervalMs"))
                );

            var endpoint = context.HttpContext.GetEndpoint();
            var routeIdAttr = endpoint?.Metadata.GetMetadata<RouteIdAttribute>();
            var routeId = routeIdAttr?.Id;

            var key = RateLimitKey.ForUserAndRoute(userId.ToString(), routeId);
            var result = await _rateLimiter.CheckAsync(key, policy, default);
            if(!result)
            {
                context.Result = new ContentResult
                {
                    Content = "Rate limit exceeded",
                    StatusCode = 429
                };
                return;
            }

            await next();
        }
    }
}
