using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.Authentication
{
    public record SessionId
    (
        Guid Id,
        int UserId
    );
}
