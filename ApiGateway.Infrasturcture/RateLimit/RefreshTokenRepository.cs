using ApiGateway.Application.Abstractions;
using ApiGateway.Doamin.Auth;
using ApiGateway.Infrastructure.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Infrastructure.RateLimit
{
    public class RefreshTokenRepository : IRefreshTokenRestore
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken?> GetAsync(CancellationToken ct, string token = null, Guid? familyId = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var entity = await _dbContext.RefreshToken
                    .AsNoTracking()
                    .Where(rt => rt.HashedToken == token)
                    .Select(t =>
                    new RefreshToken(
                        t.ID,
                        t.HashedToken,
                        t.FamilyId,
                        t.ParentID,
                        t.UserId,
                        t.ExpiresAt,
                        t.Status != 1,
                        string.Empty
                        ))
                    .FirstOrDefaultAsync(ct);

                return entity;
            }

            if (familyId is not null)
            {
                var entity = await _dbContext.RefreshToken
                    .AsNoTracking()
                    .Where(rt => rt.FamilyId == familyId)
                    .Select(t =>
                    new RefreshToken(
                        t.ID,
                        t.HashedToken,
                        t.FamilyId,
                        t.ParentID,
                        t.UserId,
                        t.ExpiresAt,
                        t.Status != 1,
                        t.RevokedReason
                        ))
                    .FirstOrDefaultAsync(ct);

                return entity;
            }
            return null;
        }

        public async Task RevokeFamilyAsync(Guid familyId, CancellationToken ct)
        {
            var familtyTokens = _dbContext.RefreshToken.Where(t => t.FamilyId == familyId);
            foreach (var token in familtyTokens)
            {
                token.Status = 2; // 標記為已撤銷
            }

            try
            {
                await _dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {

                throw new InvalidOperationException("Concurrency conflict occurred while revoking tokens.");
            }
        }

        public async Task<Result> SaveAsync(RefreshToken token, CancellationToken ct)
        {
            var addData = new Persistence.EFCore.Models.RefreshToken()
            {
                HashedToken = token.HashedToken,
                FamilyId = token.FamilyId,
                UserId = token.UserId,
                ExpiresAt = token.ExpiresAt,
                Status = token.Revoked ? 2 : 1, // 1: Active, 2: Revoked
                RevokedReason = token.Reason,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.RefreshToken.Add(addData);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when(IsUniqueConstraintViolation(ex))
            {
                return Result.Fail(new Error("RefresTokenRestore.Duplicate", "A token with the same HashedToken already exists.", ErrorType.Validation));
            }
            return Result.Ok();
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("UNIQUE") == true
                || ex.InnerException?.Message.Contains("HashedToken") == true;
        }
    }
}
