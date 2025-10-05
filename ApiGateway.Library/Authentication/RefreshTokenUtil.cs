using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Authentication;
using ApiGateway.Infrastructure.Persistence;
using Sushi.All.Infrastructure;

namespace ApiGateway.Infrastructure.Authentication
{
    public class RefreshTokenUtil
    {

        private readonly IRefreshTokenRestore _refreshTokenRepository;

        public RefreshTokenUtil(IRefreshTokenRestore refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<string> GenerateRefreshToken(int userId, string familyId)
        {
            // 產token
            Span<byte> bytes = stackalloc byte[32]; // 256 bits
            RandomNumberGenerator.Fill(bytes);

            var token = WebUtility.UrlEncode(Base64UrlHelper.Encode(bytes.ToArray()));

            // 儲存到DB
            var hashToken = EncryptHelper.ComputeSha256Hash(token);
            var resoreToken = new RefreshToken(
                TokenId: 0, // for insert
                HashedToken: hashToken,
                FamilyId: string.IsNullOrEmpty(familyId) ? Guid.NewGuid().ToString() : familyId,
                UserId: userId,
                ExpiresAt: DateTime.UtcNow.AddDays(7),
                Revoked: false,
                Reason: "Initial"
            );

            var restoreTask = _refreshTokenRepository.SaveAsync(resoreToken, default);
            await restoreTask;

            return token;
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
            // hash token
            var hashToken = EncryptHelper.ComputeSha256Hash(token);

            // db查詢驗證
            var getTask = _refreshTokenRepository.GetAsync(hashToken, default);
            var dbToken  = await getTask;

            // 驗證是否存在
            if (dbToken == null) return false;

            // 驗證是否撤銷
            if (dbToken.Revoked) return false;

            // 驗證有效期限
            if (dbToken.ExpiresAt < DateTime.Now) return false;

            return true;
        }   
    }
}
