using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Doamin.Auth;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.Abstractions
{
    public interface IRefreshTokenRestore
    {
        Task<Result> SaveAsync(RefreshToken token, CancellationToken ct);
        Task<RefreshToken?> GetAsync(CancellationToken ct, string token = null, Guid? familyId = null);
        Task RevokeFamilyAsync(Guid familyId, CancellationToken ct);
    }
}
