using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.All.Infrastructure.Result
{
    public sealed record Error(
        string Code,            // 具可溯性的错误码，如 "User.NotFound"
        string Message,         // 面向开发者/日志的说明（UI 的本地化在外层做）
        ErrorType Type,
        IReadOnlyDictionary<string, object?>? Metadata = null // 额外情报(字段名、当前版本等)
    )
    {
        public static readonly Error None = new("", "", ErrorType.None);
    }

}
