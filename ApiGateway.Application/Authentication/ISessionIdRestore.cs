using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.Authentication
{
    public interface ISessionIdRestore
    {
        Task SaveSessionId(Guid sessionId, int userId, CancellationToken ct);

        Task<SessionId> GetSessionId(Guid sessionId);
    }

}
