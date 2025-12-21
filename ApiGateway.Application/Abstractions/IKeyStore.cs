using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Application.Abstractions
{
    public interface IKeyStore
    {
        string GetKey(string keyName);
    }
}
