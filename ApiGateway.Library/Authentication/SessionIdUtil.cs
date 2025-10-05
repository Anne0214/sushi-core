using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Authentication;

namespace ApiGateway.Infrastructure.Authentication
{
    public class SessionIdUtil
    {
        private readonly ISessionIdRestore _repo;
        public SessionIdUtil(ISessionIdRestore repo)
        {
            _repo = repo;
        }
        public async Task<Guid> GenerateSessionId(int userId)
        {
            // 產生SID
            var sid = Guid.NewGuid();

            // 儲存SID
            await _repo.SaveSessionId(sid, userId, default);

            return sid;
        }

        public async Task<SessionId> ValidateSessionId(Guid sessionId)
        {
            // 查詢資料
            var userData = await _repo.GetSessionId(sessionId);

            // 返回結果
            return userData;
        }
    }
}
