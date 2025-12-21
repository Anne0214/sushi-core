using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Domain.RateLimit;

namespace ApiGateway.Application.Abstractions
{
    public interface IRateLimiter
    {
        /// <summary>嘗試消耗 1 個令牌，成功返回允許與剩餘數；若失敗，返回需等待毫秒。</summary>
        Task<bool> CheckAsync(RateLimitKey key, RateLimitPolicy policy, CancellationToken ct);
    }
}
