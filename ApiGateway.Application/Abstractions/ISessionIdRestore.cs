using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Domain.Auth;

namespace ApiGateway.Application.Abstractions
{
    public interface ISessionIdRestore
    {
        Task SaveSessionId(Guid sessionId, long userId, CancellationToken ct);

        Task<SessionId> GetSessionId(Guid sessionId);
    }

}
