using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Infrastructure.Auth;
using ApiGateway.Infrastructure.Persistence.EFCore;
using ApiGateway.Infrastructure.RateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace ApiGateway.Infrastructure.Dependency
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            #region AppDbContext注入
            var connStr = config.GetConnectionString("Default");

            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlServer(connStr, sql =>
                {
                    // 指定 Migrations 所在組件（通常放在 Infrastructure）
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    // 啟用連線重試（短暫性錯誤保護）
                    sql.EnableRetryOnFailure(maxRetryCount: 5);
                });

            });

            #endregion

            #region redis注入

            var redisConnStr = config.GetValue<string>("Redis:ConnectionString");
            // 或使用 ConfigurationOptions 詳細設定
            var options = ConfigurationOptions.Parse(redisConnStr);
            options.AbortOnConnectFail = false;   // 建議 false，避免短暫網路問題造成永久失敗
            options.ConnectRetry = 3;
            options.ConnectTimeout = 5000;
            options.SyncTimeout = 5000;
            options.KeepAlive = 180;
            // options.Ssl = true; options.Password = "xxx"; // 如需
            #endregion

            // 同步建立並注入 singleton（簡單且常見）
            var mux = ConnectionMultiplexer.Connect(options);
            services.AddSingleton<IConnectionMultiplexer>(mux);
            services.AddScoped<ISessionIdRestore, SessionIdRepository>();
            services.AddScoped<IRefreshTokenRestore, RefreshTokenRepository>();
            services.AddScoped<IRateLimiter, RedisRateLimiter>();
            services.AddScoped<IKeyStore, KeyStore>();

            return services;
        }
    }
}
