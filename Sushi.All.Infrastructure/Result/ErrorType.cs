using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.All.Infrastructure.Result
{
    public enum ErrorType
    {
        None = 0,
        Validation,
        NotFound,
        Conflict,
        Concurrency,
        Unauthorized,
        Forbidden,
        BusinessRule, // 违反领域规则但非并发/冲突
        Unexpected     // 未分类或不可预期
    }
}
