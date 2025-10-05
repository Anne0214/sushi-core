using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.Authentication
{
    public record RefreshToken
    (
        int TokenId,
        string HashedToken, 
        string FamilyId, 
        int UserId, 
        DateTime ExpiresAt, 
        bool Revoked,
        string Reason
    );
}
