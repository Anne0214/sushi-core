using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.RateLimit
{
    public interface IGetTokenBucketInfo
    {
        Task<TokenBucketInfo> GetTokenBucketInfoAsync(string apiKey, CancellationToken ct);

        Task UpdateBucketInfoAsync(TokenBucketInfo tokenBucketInfo);
    }
}
