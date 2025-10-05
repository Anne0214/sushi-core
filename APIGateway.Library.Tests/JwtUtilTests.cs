using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Authentication;
using Xunit.Sdk;

namespace APIGateway.Infrastructure.Tests
{
    public class JwtUtilTests
    {
        private readonly JwtSecurityToken _jwt;

        public JwtUtilTests()
        {
            // arrange
            var jwtUtil = new JwtUtil();
            var handler = new JwtSecurityTokenHandler();

            // act
            var token = jwtUtil.GenerateJwtToken("test", "test", 0, new TimeSpan(0,3,0));
            _jwt = handler.ReadJwtToken(token);

        }

        #region 產Token測試
        /// <summary>
        /// 產生正確的token
        /// </summary>
        [Fact]
        public void GenerateToken_Header_ShouldContainExpectedAlgAndTyp()
        {
            // assert
            Assert.Equal("HS256",_jwt.Header.Alg);
            Assert.Equal("JWT", _jwt.Header.Typ);
        }

        [Fact]
        public void GenerateTokne_TimeClaims_ShouldBeWithinTolerance()
        {
            // arrange
            var jwtUtil = new JwtUtil();
            var handler = new JwtSecurityTokenHandler();

            // act
            var token = jwtUtil.GenerateJwtToken("test", "test", 0, new TimeSpan(0, 3, 0));
            var jwt = handler.ReadJwtToken(token);

            // assert
            Assert.InRange(jwt.Payload.Expiration.Value-DateTimeOffset.UtcNow.AddMinutes(3).ToUnixTimeSeconds(),-2,2);
        }

        [Fact]
        public void GenerateToken_PayloadClaims_ShouldContainExpectedValues()
        {
            // assert
            Assert.Equal("test", _jwt.Payload.Iss);
            Assert.Equal("test", _jwt.Payload["sub"]);
            Assert.Equal("0", _jwt.Payload["userId"]);
        }

        #endregion

        #region 驗Token測試

        #endregion


        #region 測試用產token方法
        public string CreateHs256(object header, object payload, byte[] secret)
        {
            string Base64Url(string s) =>
                Convert.ToBase64String(Encoding.UTF8.GetBytes(s))
                    .TrimEnd('=').Replace('+', '-').Replace('/', '_');

            string Sign(byte[] data, byte[] key)
            {
                using var hmac = new HMACSHA256(key);
                var sig = hmac.ComputeHash(data);
                return Convert.ToBase64String(sig)
                    .TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }

            var headerJson = JsonSerializer.Serialize(header);
            var payloadJson = JsonSerializer.Serialize(payload);

            var headerB64 = Base64Url(headerJson);
            var payloadB64 = Base64Url(payloadJson);

            var signingInput = Encoding.UTF8.GetBytes($"{headerB64}.{payloadB64}");
            var sigB64 = Sign(signingInput, secret);
            return $"{headerB64}.{payloadB64}.{sigB64}";
        }

        public string CreateHs256(
            IDictionary<string, object> payload, byte[] secret,
            IDictionary<string, object>? header = null)
        {
            header ??= new Dictionary<string, object> { ["alg"] = "HS256", ["typ"] = "JWT" };
            return CreateHs256(header!, payload, secret);
        }
        #endregion
    }
}
