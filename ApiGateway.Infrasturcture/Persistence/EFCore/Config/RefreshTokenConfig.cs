using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure.Persistence.EFCore.Config
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RefreshToken> builder)
        {
            var provider = builder.Metadata.Model.GetAnnotation("ProviderName")?.ToString() ?? "";
            builder.ToTable("Auth_RefreshToken");           // 實體 → Table
            builder.HasIndex(p => p.HashedToken)
                   .HasDatabaseName("IX_Auth_RefreshToken_HashedToken"); // 指定索引名稱

            builder.HasKey(x => x.ID).HasName("PK_Auth_RefreshToken").IsClustered(true); ;
            builder.Property(x => x.ID)
              .ValueGeneratedOnAdd() // 自動產生（識別欄）
              .IsRequired();
            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.HashedToken)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(x => x.HashedToken)
                 .HasDatabaseName("IX_Auth_RefreshToken_HashedToken")
                 .IsUnique();

            builder.Property(x => x.FamilyId)
                .IsRequired();
            builder.Property(x => x.ParentID)
               .HasDefaultValue(0);

            var issuedAt = builder.Property(x => x.IssuedAt)
                                .IsRequired();
            if (provider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                issuedAt.HasDefaultValueSql("GETUTCDATE()");
            }
            else if (provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                issuedAt.HasDefaultValueSql("CURRENT_TIMESTAMP");
            }

            builder.Property(x => x.ExpiresAt)
                .IsRequired();
            builder.Property(x => x.Status)
                .HasDefaultValue(1);
            builder.Property(x => x.RevokedReason)
                .HasDefaultValue("")
                .HasColumnType("NVARCHAR(10)");
            builder.Property(x => x.Version)
                .IsConcurrencyToken();
        }
    }
}
