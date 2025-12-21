using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.DTO.MemberServiceClient
{
    public class CheckAccountAndPwdRequestDto
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
