using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGateway.Application.Abstractions;
using ApiGateway.Domain.Auth;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Application.Auth
{
    public class JwtUtil
    {
        private readonly IKeyStore _keyStore;
        public JwtUtil(IKeyStore keyStore) => (_keyStore) = (keyStore);

        /// <summary>
        /// 產生JWT Token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="sub"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GenerateJwtToken(string issuer, JwtClaim claim, TimeSpan tokenLifetime)
        {
            var handler = new JwtSecurityTokenHandler();
            DateTime expires = DateTime.UtcNow.Add(tokenLifetime);
            var key = _keyStore.GetKey("JwtKey");

            SigningCredentials signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
                );

            var claims = claim.Claims;
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
            var key = _keyStore.GetKey("JwtKey");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = validIssuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
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
