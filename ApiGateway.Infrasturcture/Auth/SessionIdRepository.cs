using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Domain.Auth;
using ApiGateway.Infrastructure.Persistence.EFCore;
using ApiGateway.Infrastructure.Persistence.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure.Auth
{
    public class SessionIdRepository : ISessionIdRestore
    {
        private readonly AppDbContext _dbContext;

        public SessionIdRepository(AppDbContext dbContext) => (_dbContext) = (dbContext);

        public async Task<Domain.Auth.SessionId> GetSessionId(Guid sessionId)
        {
            var result =  await _dbContext.SessionId
                    .AsNoTracking()
                    .Where(s => s.SID == sessionId)
                    .FirstOrDefaultAsync();
            return new Domain.Auth.SessionId(result.SID, result.UserID);
                    
        }

        public async Task SaveSessionId(Guid sessionId, long userId, CancellationToken ct)
        {
            var sessionIdData = new Persistence.EFCore.Models.SessionId
            {
                SID = sessionId,
                UserID  = userId
            };

            _dbContext.SessionId.Add(sessionIdData);
            await _dbContext.SaveChangesAsync();
        }
    }
}
