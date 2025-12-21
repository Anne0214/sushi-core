using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ApiGateway.Infrastructure.Auth
{
    public class KeyStore : IKeyStore
    {
        private readonly IConfiguration _config;
        public KeyStore(IConfiguration config)
        {
            _config = config;
        }
        public string GetKey(string keyName)
        {
            return _config.GetValue<string>($"KeyStore:{keyName}");
        }
    }
}
