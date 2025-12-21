using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Domain.RateLimit;
using StackExchange.Redis;

namespace ApiGateway.Infrastructure.RateLimit
{
    public class RedisRateLimiter: IRateLimiter
    {
        private readonly IConnectionMultiplexer _conn;
        public RedisRateLimiter(IConnectionMultiplexer connection) => (_conn) = (connection);
        public async Task<bool> CheckAsync(RateLimitKey key, RateLimitPolicy policy, CancellationToken ct)
        {
            var db = _conn.GetDatabase();

            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Caching", "token_bucket.lua");
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Lua script not found: {scriptPath}");

            var script = File.ReadAllText(scriptPath);
            var prepared = LuaScript.Prepare(script);
            var parameters = new
            {
                bucket = (RedisKey)key.Value, // RedisKey -> KEYS
                maxCapacity = policy.Capacity,            // 其餘 -> ARGV
                refillIntervalMs = policy.RefillIntervalMs,
                refillTokens = policy.RefillTokens,
                nowMs = 0                    // <=0 代表用 redis TIME
            };

            RedisResult result = await db.ScriptEvaluateAsync(prepared, parameters);
            return (int)result == 1;
        }
    }
}
