using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Domain.RateLimit
{
    /// <summary>
    /// capacity: 桶容量；refillTokens: 每個 refillIntervalMs 補充的令牌數。
    /// </summary>
    public sealed record RateLimitPolicy(
        int Capacity,
        int RefillTokens,
        int RefillIntervalMs
    );
}
