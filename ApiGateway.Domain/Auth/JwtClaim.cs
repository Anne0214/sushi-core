using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Domain.Auth
{
    public sealed record JwtClaim(long UserId)
    {
        public bool IsValid() => UserId > 100000;
        
        public Claim[] Claims => new[]
        {
            new Claim("userId", UserId.ToString())
        };
    }
}
