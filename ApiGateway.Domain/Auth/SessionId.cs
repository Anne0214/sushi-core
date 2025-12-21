using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Domain.Auth
{
    public record SessionId
    (
        Guid Id,
        long UserId
    );
}
