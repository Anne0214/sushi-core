using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Doamin.Auth
{
    public record RefreshToken
    (
        int TokenId,
        string HashedToken, 
        Guid FamilyId, 
        int ParentId,
        long UserId, 
        DateTime ExpiresAt, 
        bool Revoked,
        string Reason
    );
}
