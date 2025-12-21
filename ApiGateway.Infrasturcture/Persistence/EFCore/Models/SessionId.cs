using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore.Abstracts;

namespace ApiGateway.Infrastructure.Persistence.EFCore.Models
{
    public class SessionId : IHasAudit
    {
        public Guid SID { get; set; }
        public long UserID { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long Version { get; private set; } = 1;
    }
}
