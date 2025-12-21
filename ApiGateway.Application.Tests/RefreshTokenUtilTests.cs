using System;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Auth;
using ApiGateway.Doamin.Auth;
using Moq;
using Sushi.All.Infrastructure;
using Sushi.All.Infrastructure.Result;

namespace APIGateway.Infrastructure.Tests
{
    public class RefreshTokenUtilTests
    {
        #region GenerateRefreshToken
        [Fact]
        public async Task GenerateRefreshToken_ShouldReturnBase64String()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var task = refreshTokenUtil.GenerateRefreshToken(0, null);
            var result = await task;

            // assert
            Assert.NotNull(result.Value);
            Assert.True(result.Value.Length >= 43); // Base64 string length for 32 bytes is at least 43 characters
            Assert.DoesNotContain("=", result.Value); // Ensure no padding characters
        }

        [Fact]
        public void GenerateRefreshToken_ShouldRestoreToDb()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, null);

            // assert
            repoMock.Verify(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldUseProvidedFamilyId()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);
            var familyId = Guid.NewGuid();

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, familyId);

            // assert
            repoMock.Verify(r => r.SaveAsync(It.Is<RefreshToken>(t => t.FamilyId == familyId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldUseProvidedUserId()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok()); var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, null);

            // assert
            repoMock.Verify(r => r.SaveAsync(It.Is<RefreshToken>(t => t.UserId == 0), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GenerateRefreshToken_CreateNewFamilyId_WhenNotProvided()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, null);

            // assert
            repoMock.Verify(r => r.SaveAsync(It.Is<RefreshToken>(t => t.FamilyId!= Guid.Empty), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GenerateRefreshToken_DuplicateTokenWouldRetry()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(t => t.SaveAsync(It.IsAny<RefreshToken>(),It.IsAny<CancellationToken>())).ReturnsAsync(Result.Fail(new Error("RefresTokenRestore.Duplicate","",ErrorType.Validation)));

            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, null);

            // assert
            repoMock.Verify(r => r.SaveAsync(It.Is<RefreshToken>(t => t.FamilyId != Guid.Empty), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateRefreshToken_WithExistFamilyId_RestoreWithParentIDAndRevokeLastOne()
        {

            // arrange
            var familyId = Guid.NewGuid();
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            repoMock.Setup(r => r.GetAsync(It.IsAny<CancellationToken>(), null, It.Is<Guid>(t => t == familyId))).ReturnsAsync( new RefreshToken(1,"hashedToken",familyId,0,1,DateTime.UtcNow.AddDays(7), false, string.Empty));
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, familyId);

            // assert
            repoMock.Verify(r => r.GetAsync(It.IsAny<CancellationToken>(), It.IsAny<string>(), It.Is<Guid>(x => x == familyId)), Times.Once);

            repoMock.Verify(r => r.SaveAsync(It.Is<RefreshToken>(t => t.FamilyId == familyId && t.ParentId == 1), It.IsAny<CancellationToken>()), Times.Once);

            repoMock.Verify(r => r.RevokeFamilyAsync(It.Is<Guid>(f => f == familyId), It.IsAny<CancellationToken>()),Times.Once);
        }
        #endregion

        #region ValidateRefreshToken
        [Fact]
        public async Task ValidateRefreshToken_ShouldSearchToken()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            var token = "testToken";
            var hashToken = EncryptHelper.ComputeSha256Hash(token);

            // act
            var result = await refreshTokenUtil.ValidateRefreshToken(token);

            // assert
            repoMock.Verify(r => r.GetAsync(It.IsAny<CancellationToken>(),It.Is<string>(t => t == hashToken),null), Times.Once);
        }

        [Fact]
        public void ValidateRefreshToken_ShouldReturnFalse_WhenTokenNotFound()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.GetAsync(It.IsAny<CancellationToken>(), It.IsAny<string>(),null ))
                .ReturnsAsync((RefreshToken?)null);
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);
            var token = "nonExistentToken";

            // act
            var task = refreshTokenUtil.ValidateRefreshToken(token);
            var result = task.Result;

            // assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateRefreshToken_ShouldReturnFalse_WhenTokenIsExpired()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.GetAsync(
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>(),
                    null
                    ))
                .ReturnsAsync(
                    new RefreshToken
                    (
                        TokenId: 1,
                        HashedToken: "hashedToken",
                        FamilyId: Guid.NewGuid(),
                        ParentId : 0,
                        UserId: 1,
                        ExpiresAt: DateTime.UtcNow.AddMinutes(-1), // expired
                        Revoked: false,
                        Reason: "Test"
                    )
                 );

            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var task = refreshTokenUtil.ValidateRefreshToken("token");
            var result = task.Result;

            // assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateRefreshToken_ShouldReturnFalse_WhenTokenIsRevoked()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.GetAsync(
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>(),
                    null
                    ))
                .ReturnsAsync(
                    new RefreshToken
                    (
                        TokenId: 1,
                        HashedToken: "hashedToken",
                        FamilyId: Guid.NewGuid(),
                        ParentId: 0,
                        UserId: 1,
                        ExpiresAt: DateTime.Now.AddMinutes(15),
                        Revoked: true,
                        Reason: "Test"
                    )
                 );

            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var task = refreshTokenUtil.ValidateRefreshToken("token");
            var result = task.Result;

            // assert
            Assert.False(result);

        }
        #endregion
    }
}
