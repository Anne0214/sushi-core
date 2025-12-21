using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIGateway.Infrastructure.Tests.Utility;
using FluentAssertions;
using StackExchange.Redis;

namespace APIGateway.Infrastructure.Tests
{
    [Collection("redis-collection")]
    public class RedisRateLimiterTests
    {
        private readonly IDatabase _db1;
        private readonly IDatabase _db2;
        private readonly LoadedLuaScript _loaded;


        public RedisRateLimiterTests(RedisFixture fx)
        {
            _db1 = fx.Conn1.GetDatabase();
            _db2 = fx.Conn2.GetDatabase();

            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Caching", "token_bucket.lua");
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Lua script not found: {scriptPath}");

            var scriptText = File.ReadAllText(scriptPath);

            var prepared = LuaScript.Prepare(scriptText);
            // 載入到 Redis 以快取 SHA
            var server = fx.Conn1.GetServer(fx.Conn1.GetEndPoints().First());
            _loaded = prepared.Load(server);
        }

        [Fact]
        public async Task Concurrent_Requests_Should_Not_Exceed_Capacity()
        {
            var bucketKey = "bucket:test:atomic";
            await _db1.KeyDeleteAsync(bucketKey);

            int capacity = 50;
            int refillMs = 60_000; // 測試期間不補
            int refillTokens = 1;

            // 200 併發：一半連線 1，一半連線 2
            var tasks = Enumerable.Range(0, 200).Select(async i =>
            {
                var db = (i % 2 == 0) ? _db1 : _db2;
                await Task.Yield();

                var parameters = new
                {
                    bucket = (RedisKey)bucketKey, // RedisKey -> KEYS
                    maxCapacity = capacity,            // 其餘 -> ARGV
                    refillIntervalMs = refillMs,
                    refillTokens = refillTokens,
                    nowMs = 0                    // <=0 代表用 redis TIME
                };

                var res = await db.ScriptEvaluateAsync(_loaded, parameters);
                return (int)res == 1;
            });

            var results = await Task.WhenAll(tasks);
            results.Count(x => x).Should().BeLessThanOrEqualTo(capacity);
        }

        [Fact]
        public async Task Refill_Should_Work_When_Time_Moves_Forward()
        {
            var bucketKey = "bucket:test:refill";
            await _db1.KeyDeleteAsync(bucketKey);

            int capacity = 3;
            int refillMs = 1_000;
            int refillTokens = 1;

            long now = 1_000_000;

            // 先取 3 次 -> 都成功
            for (int i = 0; i < 3; i++)
            {
                var p = new
                {
                    bucket = (RedisKey)bucketKey, // RedisKey -> KEYS
                    maxCapacity = capacity,            // 其餘 -> ARGV
                    refillIntervalMs = refillMs,
                    refillTokens = refillTokens,
                    nowMs = now
                };
                var r = await _db1.ScriptEvaluateAsync(_loaded, p);
                ((int)r).Should().Be(1);
            }

            // 第 4 次同時間 -> 應拒絕
            {
                var p = new
                {
                    bucket = (RedisKey)bucketKey, // RedisKey -> KEYS
                    maxCapacity = capacity,            // 其餘 -> ARGV
                    refillIntervalMs = refillMs,
                    refillTokens = refillTokens,
                    nowMs = now
                };
                var r = await _db1.ScriptEvaluateAsync(_loaded, p);
                ((int)r).Should().Be(0);
            }

            // 時間往後推 1100ms -> 應補 1 枚
            now += 1100;
            {
                var p = new
                {
                    bucket = (RedisKey)bucketKey, // RedisKey -> KEYS
                    maxCapacity = capacity,            // 其餘 -> ARGV
                    refillIntervalMs = refillMs,
                    refillTokens = refillTokens,
                    nowMs = now
                };
                var r = await _db1.ScriptEvaluateAsync(_loaded, p);
                ((int)r).Should().Be(1);
            }

        }
    }
}
