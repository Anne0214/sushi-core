using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.RateLimit
{
    public class TokenBucketInfo
    {
        public int token;
        public DateTime lastRefillTime;
    }

}
