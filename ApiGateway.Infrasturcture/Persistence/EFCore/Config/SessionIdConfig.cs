using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiGateway.Infrastructure.Persistence.EFCore.Config
{
    public class SessionIdConfig : IEntityTypeConfiguration<SessionId>
    {
        public void Configure(EntityTypeBuilder<SessionId> builder)
        {
            builder.ToTable("Auth_SessionId");           // 實體 → Table

            builder.HasKey(x => x.SID).HasName("PK_Auth_SessionId").IsClustered(true);
            builder.Property(x => x.UserID)
                    .IsRequired();
            builder.Property(x => x.Version)
                    .IsConcurrencyToken();

        }
    }
}
