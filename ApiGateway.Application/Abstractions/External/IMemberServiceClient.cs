using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.DTO.MemberServiceClient;

namespace ApiGateway.Application.Abstractions.External
{
    public interface IMemberServiceClient
    {
        Task<long> CheckAccountAndPwd(CheckAccountAndPwdRequestDto request);
    }
}
