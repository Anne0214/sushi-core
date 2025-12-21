using System;
using System.Security.Cryptography;
using ApiGateway.Application.Abstractions;
using ApiGateway.Doamin.Auth;
using Sushi.All.Infrastructure;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.Auth
{
    public class RefreshTokenUtil
    {

        private readonly IRefreshTokenRestore _refreshTokenRepository;

        public RefreshTokenUtil(IRefreshTokenRestore refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<Result<string>> GenerateRefreshToken(long userId, Guid? familyId = null)
        {
            const int maxAttempts = 2;
            Guid fid;
            int parentId = 0;
            if (familyId is not null)
            {
                var existingToken = await _refreshTokenRepository.GetAsync(default, null, familyId);
                parentId = existingToken?.TokenId ?? 0;  
                fid = (Guid)familyId;
            }
            else
            {
                fid = Guid.NewGuid();
            }

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                // 產token
                Span<byte> bytes = stackalloc byte[32]; // 256 bits
                RandomNumberGenerator.Fill(bytes);

                var token = Base64UrlHelper.Encode(bytes.ToArray());

                // revoke last token
                await _refreshTokenRepository.RevokeFamilyAsync(fid, default);

                // 儲存到DB
                var hashToken = EncryptHelper.ComputeSha256Hash(token);
                var restoreToken = new RefreshToken(
                    TokenId: 0, // for insert
                    HashedToken: hashToken,
                    FamilyId: fid,
                    ParentId : parentId,
                    UserId: userId,
                    ExpiresAt: DateTime.UtcNow.AddDays(7),
                    Revoked: false,
                    Reason: "Initial"
                );

                var restoreResult = await _refreshTokenRepository.SaveAsync(restoreToken, default);

                if (restoreResult.IsSuccess) return Result<string>.Ok(token);

                if (restoreResult.Error.Code == "RefresTokenRestore.Duplicate")
                {
                    continue;
                }
            }

            return Result<string>.Fail(new Error("RefreshTokenUtil.Fail","",ErrorType.Unexpected));
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
            // hash token
            var hashToken = EncryptHelper.ComputeSha256Hash(token);

            // db查詢驗證
            var getTask = _refreshTokenRepository.GetAsync(default,hashToken, null);
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
