using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.All.Infrastructure
{
    public static class EncryptHelper
    {
        public static string ComputeSha256Hash(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
