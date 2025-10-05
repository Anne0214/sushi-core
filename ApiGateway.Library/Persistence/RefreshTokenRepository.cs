using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Authentication;

namespace ApiGateway.Infrastructure.Persistence
{
    public class RefreshTokenRepository : IRefreshTokenRestore
    {
        public Task<RefreshToken?> GetAsync(string token, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task RevokeFamilyAsync(string familyId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(RefreshToken token, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
