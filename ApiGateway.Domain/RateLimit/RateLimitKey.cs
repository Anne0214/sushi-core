using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Domain.RateLimit
{
    public readonly record struct RateLimitKey(string Value)
    {
        public static RateLimitKey ForUserAndRoute(string userId, string routeId)
            => new($"{userId}:{routeId}");

        public override string ToString() => Value;

    }
}
