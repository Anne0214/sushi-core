using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Abstractions.External;
using ApiGateway.Infrastructure.Auth;
using ApiGateway.Infrastructure.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.Infrastructure.Dependency
{
    public static class MemberClientInjection
    {
        public static IServiceCollection AddMemberClient(this IServiceCollection services, IConfiguration config)
        {
            var baseUri = new Uri(config.GetValue<string>("Member:Domain"));
            services.AddHttpClient<IMemberServiceClient,MemberHttpClient>(client =>
            {
                client.BaseAddress = baseUri;
                client.Timeout = TimeSpan.FromSeconds(config.GetValue<int>("Member:ConnectTimeout"));
            });

            // todo 待新增Handler例如: 重試、驗證器等
            return services;
        }
    }
}
