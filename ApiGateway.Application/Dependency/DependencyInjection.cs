using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Auth;
using ApiGateway.Application.UseCase;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.Application.Dependency
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<JwtUtil>();
            services.AddScoped<RefreshTokenUtil>();
            services.AddScoped<SessionIdUtil>();

            return services;
        }
    }
}
