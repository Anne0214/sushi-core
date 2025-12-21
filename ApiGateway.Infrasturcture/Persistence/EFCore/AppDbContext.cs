using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore.Config;
using ApiGateway.Infrastructure.Persistence.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure.Persistence.EFCore
{
    public class AppDbContext : DbContext
    {
        public string Provider { get; }
        public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();

        public DbSet<SessionId> SessionId => Set<SessionId>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Provider = Database.ProviderName?.ToLower() ?? "";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 將 ProviderName 設定到 Model Annotation
            modelBuilder.Model.SetAnnotation("ProviderName", Database.ProviderName);

            // 自動載入所有 IEntityTypeConfiguration<>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
