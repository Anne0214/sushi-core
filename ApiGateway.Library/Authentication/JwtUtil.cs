using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Infrastructure.Authentication
{
    public class JwtUtil
    {
        /// <summary>
        /// 產生JWT Token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="sub"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GenerateJwtToken(string issuer, string sub, int userId, TimeSpan tokenLifetime)
        {
            var handler = new JwtSecurityTokenHandler();
            DateTime expires = DateTime.UtcNow.Add(tokenLifetime);

            SigningCredentials signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JttfUNWwAPBa84ByQf/1aJyGcP6Y3hyj0CLlLhhWczA=")),
                SecurityAlgorithms.HmacSha256
                );

            var claims = new[] {
                new Claim("sub",sub),
                new Claim("userId",userId.ToString())
            };
            var token = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                expires: expires,
                issuer : issuer,
                claims : claims
                );

            return handler.WriteToken(token);
        }   

        /// <summary>
        /// 驗證Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="validIssuer"></param>
        /// <returns></returns>
        public ClaimsPrincipal ValidateToken(string token, string validIssuer)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = validIssuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JttfUNWwAPBa84ByQf/1aJyGcP6Y3hyj0CLlLhhWczA=")),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero for testing purposes
            };
            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception)
            {
                // Token validation failed
                return null;
            }
        }
    }
}
