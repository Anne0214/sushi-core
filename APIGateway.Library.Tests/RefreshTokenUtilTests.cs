using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Authentication;
using ApiGateway.Infrastructure.Authentication;
using Moq;
using Newtonsoft.Json.Linq;
using Sushi.All.Infrastructure;

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
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var task = refreshTokenUtil.GenerateRefreshToken(0, null);
            var token = await task;

            // assert
            Assert.NotNull(token);
            Assert.True(token.Length >= 43); // Base64 string length for 32 bytes is at least 43 characters
            Assert.DoesNotContain("=", token); // Ensure no padding characters
        }

        [Fact]
        public void GenerateRefreshToken_ShouldRestoreToDb()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
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
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);
            var familyId = Guid.NewGuid().ToString();

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
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

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
            var refreshTokenUtil = new RefreshTokenUtil(repoMock.Object);

            // act
            var token = refreshTokenUtil.GenerateRefreshToken(0, null);

            // assert
            repoMock.Verify(r => r.SaveAsync(It.Is<RefreshToken>(t => !string.IsNullOrEmpty(t.FamilyId)), It.IsAny<CancellationToken>()), Times.Once);
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
            repoMock.Verify(r => r.GetAsync(It.Is<string>(t => t == hashToken), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ValidateRefreshToken_ShouldReturnFalse_WhenTokenNotFound()
        {
            // arrange
            var repoMock = new Mock<IRefreshTokenRestore>();
            repoMock.Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new RefreshToken
                    (
                        TokenId: 1,
                        HashedToken: "hashedToken",
                        FamilyId: "familyId",
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
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new RefreshToken
                    (
                        TokenId: 1,
                        HashedToken: "hashedToken",
                        FamilyId: "familyId",
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
