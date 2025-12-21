using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Infrastructure.Persistence.EFCore.Abstracts
{
    public interface IHasAudit
    {
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
        long Version { get; }
    }
}
