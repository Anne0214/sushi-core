using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.Authentication
{
    public interface IRefreshTokenRestore
    {
        Task SaveAsync(RefreshToken token, CancellationToken ct);
        Task<RefreshToken?> GetAsync(string token, CancellationToken ct);
        Task RevokeFamilyAsync(string familyId, CancellationToken ct);
    }
}
