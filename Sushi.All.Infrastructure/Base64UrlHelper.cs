using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.All.Infrastructure
{
    using System;
    using System.Text;
    using System.Text.Json;

    public static class Base64UrlHelper
    {
        /// <summary>
        /// 編碼 byte[] → Base64Url
        /// </summary>
        public static string Encode(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            return base64
                .Replace("+", "-")   // URL safe
                .Replace("/", "_")   // URL safe
                .TrimEnd('=');       // 移除 padding
        }

        /// <summary>
        /// 解碼 Base64Url → byte[]
        /// </summary>
        public static byte[] Decode(string input)
        {
            var base64 = input
                .Replace("-", "+")
                .Replace("_", "/");

            // 補齊長度，必須是 4 的倍數
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        /// <summary>
        /// 編碼字串 → Base64Url
        /// </summary>
        public static string EncodeString(string input, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return Encode(encoding.GetBytes(input));
        }

        /// <summary>
        /// 解碼 Base64Url → 字串
        /// </summary>
        public static string DecodeToString(string input, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = Decode(input);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 編碼物件 → JSON → Base64Url
        /// </summary>
        public static string EncodeObject<T>(T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return EncodeString(json);
        }

        /// <summary>
        /// 解碼 Base64Url → JSON → 物件
        /// </summary>
        public static T? DecodeObject<T>(string input)
        {
            var json = DecodeToString(input);
            return JsonSerializer.Deserialize<T>(json);
        }
    }

}
