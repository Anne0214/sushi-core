using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore;
using ApiGateway.Infrastructure.Persistence.EFCore.Interceptors;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace APIGateway.Infrastructure.Tests.Utility
{
    public sealed class SqliteInMemoryDb : IAsyncDisposable
    {
        public SqliteConnection Connection { get; }
        public DbContextOptions<AppDbContext> Options { get; }

        public SqliteInMemoryDb()
        {
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open(); // 連線不關，資料庫才活著

            var interceptor = new UpdateAuditInterceptor();

            Options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(Connection)
                .AddInterceptors(interceptor)
                .Options;

            // 建立資料表
            using var db = new AppDbContext(Options);
            db.Database.EnsureCreated();
        }
        public ValueTask DisposeAsync()=>Connection.DisposeAsync();
    }
}
