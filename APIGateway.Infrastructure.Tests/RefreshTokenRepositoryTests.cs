using System;
using ApiGateway.Infrastructure.Persistence.EFCore;
using ApiGateway.Infrastructure.Persistence.EFCore.Models;
using ApiGateway.Infrastructure.RateLimit;
using APIGateway.Infrastructure.Tests.Utility;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace APIGateway.Infrastructure.Tests
{
    public class RefreshTokenRepositoryTests
    {
        
        #region GetAsync
        [Fact]
        public async Task GetAsync_NoneExsistentToken_ReturnNull()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();

            // act
            var result = await repo.GetAsync(cts,"none-Exsistent");

            // assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetAsync_ExsistentToken_ReturnToken()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();

            ctx.RefreshToken.Add(new RefreshToken
            {
                ID = 1,
                HashedToken = "ExistToken",
                UserId = 1,
                ParentID = 0,
                FamilyId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IssuedAt = DateTime.UtcNow,
                RevokedReason = string.Empty,
                Status = 1
            });

            await ctx.SaveChangesAsync();

            // act
            var result = await repo.GetAsync(cts, "ExistToken");

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_ExistFamilyId_ReturnToken()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();
            Guid fid = Guid.NewGuid();

            ctx.RefreshToken.Add(new RefreshToken
            {
                ID = 1,
                HashedToken = "ExistToken",
                UserId = 1,
                ParentID = 0,
                FamilyId = fid,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IssuedAt = DateTime.UtcNow,
                RevokedReason = string.Empty,
                Status = 1
            });

            await ctx.SaveChangesAsync();

            // act
            var result = await repo.GetAsync(cts, null, fid);

            // assert
            Assert.NotNull(result);

        }

        [Fact]
        public async Task GetAsync_NonExistentFamilyId_ReturnNull()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();
            Guid fid = Guid.NewGuid();

            await ctx.SaveChangesAsync();

            // act
            var result = await repo.GetAsync(cts, null, fid);

            // assert
            Assert.Null(result);
        }

        // 只會返回同familyID中最新的一個

        #endregion

        #region RevokeFamilyAsync
        [Fact]
        public async Task RevokeFamilyAsync_RevokeSuccess()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();
            var familyId = Guid.NewGuid();

            ctx.RefreshToken.Add(new RefreshToken
            {
                ID = 1,
                HashedToken = "ExistToken",
                UserId = 1,
                ParentID = 0,
                FamilyId = familyId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IssuedAt = DateTime.UtcNow,
                RevokedReason = string.Empty,
                Status = 1
            });

            await ctx.SaveChangesAsync();

            // act & Assert
            await repo.RevokeFamilyAsync(familyId, cts);
            var resultStatus = ctx.RefreshToken.AsNoTracking().Where(t => t.FamilyId == familyId).FirstOrDefault().Status;

            Assert.Equal(resultStatus,2);
        }

        [Fact]
        public async Task RevokeFamilyAsync_NoneExistentFamilyId_RevokeSuccess()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();
            var familyId = Guid.NewGuid(); //資料庫不存在的familyId


            // act & Assert
            await repo.RevokeFamilyAsync(familyId, cts);

            var exception = await Record.ExceptionAsync(async ()=>await repo.RevokeFamilyAsync(familyId, cts));

            Assert.Null(exception);
        }
        #endregion

        #region SaveAsync
        [Fact]
        public async Task SaveAsync_SaveSuccess()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();
            var fid = Guid.NewGuid();
            var expireAt = DateTime.Now.AddHours(7);

            var token = new ApiGateway.Doamin.Auth.RefreshToken(1,"hashedToken", fid, 0, 1, expireAt, false,"");
            var result = await repo.SaveAsync(token, cts);

            var dbResult = ctx.RefreshToken.Where(t => t.ID == 1 && t.HashedToken == "hashedToken" && t.FamilyId == fid && t.ExpiresAt == expireAt && t.RevokedReason == "" && t.Status == 1).FirstOrDefault();

            Assert.True(result.IsSuccess);
            Assert.NotNull(dbResult);
        }

        [Fact]
        public async Task SaveAsync_DuplicateHashedToken_ReturnFail()
        {
            // arrange
            await using var db = new SqliteInMemoryDb();
            await using var ctx = new AppDbContext(db.Options);
            await ctx.Database.EnsureDeletedAsync();
            await ctx.Database.EnsureCreatedAsync();
            var repo = new RefreshTokenRepository(ctx);
            var cts = new CancellationToken();
            var fid = Guid.NewGuid();
            var expireAt = DateTime.Now.AddHours(7);
            string tokenContent = "hashedToken";

            var dbToken = new RefreshToken()
            {
                HashedToken = tokenContent,
                UserId = 1,
                FamilyId = fid,
                ParentID = 1,
                ExpiresAt = expireAt,
                IssuedAt = DateTime.Now,
                RevokedReason = "",
                Status = 1
            };
            var token = new ApiGateway.Doamin.Auth.RefreshToken(0, tokenContent, fid, 0, 1, expireAt, false, "");

            ctx.RefreshToken.Add(dbToken);
            ctx.SaveChanges();
            var result = await repo.SaveAsync(token, cts);

            Assert.False(result.IsSuccess);
            Assert.True(result.Error.Code == "RefresTokenRestore.Duplicate");

        }
        #endregion
    }
}
