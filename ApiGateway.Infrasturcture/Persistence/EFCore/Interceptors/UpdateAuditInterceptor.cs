using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ApiGateway.Infrastructure.Persistence.EFCore.Interceptors
{
    public class UpdateAuditInterceptor :SaveChangesInterceptor
    {

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var dbContext = eventData.Context;
            ApplyAuditLogic(dbContext);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;
            ApplyAuditLogic(dbContext);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplyAuditLogic(DbContext context)
        {
            var now = DateTimeOffset.UtcNow;
            foreach (var e in context.ChangeTracker.Entries<IHasAudit>())
            {
                if (e.State == EntityState.Added)
                {
                    e.Property("CreatedAt").CurrentValue = now;
                    e.Property("UpdatedAt").CurrentValue = now;
                    e.Property("Version").CurrentValue = 0L;
                }
                else if (e.State == EntityState.Modified)
                {
                    e.Property("UpdatedAt").CurrentValue = now;
                    var oldVersion = (long)e.Property("Version").CurrentValue!;
                    e.Property("Version").CurrentValue = oldVersion + 1;
                }
            }
        }
    }
}
