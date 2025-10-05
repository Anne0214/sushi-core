using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.RateLimit;

namespace ApiGateway.Infrastructure.RateLimit
{
    public class TokenBucketUtil
    {
        private readonly IGetTokenBucketInfo _repo;
        private const string TokenBucketKey = "TokenBucket";
        private const int MaxTokens = 10;
        private const int RefillRate = 1; // tokens per second
        private readonly TimeProvider _timeProvider;

        public TokenBucketUtil(IGetTokenBucketInfo repo, TimeProvider timeProvider)
        {
            _repo = repo;
            _timeProvider = timeProvider;
        }

        public async Task<bool> CheckTokenBucketAmount()
        {
            // 取得token資訊
            var tokenInfo = await _repo.GetTokenBucketInfoAsync(TokenBucketKey, default);

            // 補充token
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var secondesElapsed = (now - tokenInfo.lastRefillTime).Seconds;
            var tokensToAdd = secondesElapsed * RefillRate;
            tokenInfo.token = Math.Min(MaxTokens, tokenInfo.token + tokensToAdd);

            if (tokenInfo.token >= 1)
            {
                tokenInfo.lastRefillTime = now;
                tokenInfo.token -= 1;
                await _repo.UpdateBucketInfoAsync(tokenInfo);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
