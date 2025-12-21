using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace APIGateway.Infrastructure.Tests.Utility
{
    public sealed class RedisFixture : IAsyncLifetime
    {
        public RedisContainer Container { get; private set; } = default!;
        public ConnectionMultiplexer Conn1 { get; private set; } = default!;
        public ConnectionMultiplexer Conn2 { get; private set; } = default!; // 另一條連線模擬多進程

        public async Task InitializeAsync()
        {
            Container = new RedisBuilder()
                .WithImage("redis:7.2-alpine")
                .Build();

            await Container.StartAsync();

            var host = Container.Hostname;
            var port = Container.GetMappedPublicPort(6379);
            var connStr = $"{host}:{port},abortConnect=false";

            Conn1 = await ConnectionMultiplexer.ConnectAsync(connStr);
            Conn2 = await ConnectionMultiplexer.ConnectAsync(connStr);
        }

        public async Task DisposeAsync()
        {
            if (Conn1 is not null) await Conn1.DisposeAsync();
            if (Conn2 is not null) await Conn2.DisposeAsync();
            if (Container is not null) await Container.DisposeAsync();
        }
    }

    [CollectionDefinition("redis-collection")]
    public class RedisCollection : ICollectionFixture<RedisFixture> { }
}
