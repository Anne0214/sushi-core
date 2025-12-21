using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.UseCase;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.Abstractions
{
    public interface ILoginUseCase
    {
        public Task<Result<LoginRes>> ExecuteAsync(string account, string password, string gatewayName, int lifeTimeHour);
    }
}
