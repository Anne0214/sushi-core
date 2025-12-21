using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Abstractions.External;
using ApiGateway.Application.Auth;
using ApiGateway.Application.UseCase;
using ApiGateway.Doamin.Auth;
using Moq;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.Tests.UseCase
{
    public class LoginUseCaseTests
    {
        private void Setup()
        {
        
        }

        // 整體流程成功，返回 tokens
        [Fact]
        public  async void LoginUseCase_ShouldReturnTokens_WhenPwdAreValid()
        {
            // arrange
            var memberServiceMock = new Mock<IMemberServiceClient>();
            memberServiceMock
                .Setup(m => m.CheckAccountAndPwd(It.IsAny<DTO.MemberServiceClient.CheckAccountAndPwdRequestDto>()))
                .ReturnsAsync(100000);

            var keyStoreMock = new Mock<IKeyStore>();
            keyStoreMock.Setup(k => k.GetKey(It.IsAny<string>()))
                .Returns("b9e7ac4f45c67e6b12f0c3e0c87279f56f50dcf6a7e44965d0f2a46ec3cda87b");

            var jwtUtilMock = new JwtUtil(keyStoreMock.Object);

            var refreshTokenRepo = new Mock<IRefreshTokenRestore>();
            refreshTokenRepo.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())).ReturnsAsync( Result.Ok());

            var refreshTokenUtil = new RefreshTokenUtil(refreshTokenRepo.Object);
            var useCase = new LoginUseCase(memberServiceMock.Object, jwtUtilMock, refreshTokenUtil);

            // act
            var result = await useCase.ExecuteAsync("UnitTest","validAccount", "validPassword",3);

            //assert
            Assert.NotNull(result.Value.RefreshToken);
            Assert.NotNull(result.Value.AccessToken);
        }

        // member返回驗證失敗，返回登入失敗
        [Fact]
        public async void LoginUseCase_ShouldReturnError_WhenMemberValidationFails()
        {
            // arrange
            var memberServiceMock = new Mock<IMemberServiceClient>();
            memberServiceMock
                .Setup(m => m.CheckAccountAndPwd(It.IsAny<DTO.MemberServiceClient.CheckAccountAndPwdRequestDto>()))
                .ReturnsAsync(0);

            var keyStoreMock = new Mock<IKeyStore>();
            keyStoreMock.Setup(k => k.GetKey(It.IsAny<string>()))
                .Returns("b9e7ac4f45c67e6b12f0c3e0c87279f56f50dcf6a7e44965d0f2a46ec3cda87b");

            var jwtUtilMock = new JwtUtil(keyStoreMock.Object);

            var refreshTokenRepo = new Mock<IRefreshTokenRestore>();
            refreshTokenRepo.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Ok());

            var refreshTokenUtil = new RefreshTokenUtil(refreshTokenRepo.Object);
            var useCase = new LoginUseCase(memberServiceMock.Object, jwtUtilMock, refreshTokenUtil);

            // act
            var result = await useCase.ExecuteAsync("UnitTest", "invalidAccount", "invalidPassword", 3);

            // assert
            Assert.Equal(result.Error.Code, "LoginUseCase.InvalidAccountOrPassword");
        }

        // access token 產生失敗，返回系統有問題，請稍後重試
        [Fact]
        public async void LoginUseCase_ShouldReturnSystemError_WhenAccessTokenGenerationFails()
        {
            // arrange
            var memberServiceMock = new Mock<IMemberServiceClient>();
            memberServiceMock
                .Setup(m => m.CheckAccountAndPwd(It.IsAny<DTO.MemberServiceClient.CheckAccountAndPwdRequestDto>()))
                .ReturnsAsync(100000);
            var keyStoreMock = new Mock<IKeyStore>();
            keyStoreMock.Setup(k => k.GetKey(It.IsAny<string>()))
                .Returns(string.Empty); // 模擬取得無效的金鑰

            var jwtUtilMock = new JwtUtil(keyStoreMock.Object);
            var refreshTokenRepo = new Mock<IRefreshTokenRestore>();
            refreshTokenRepo.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Ok());
            var refreshTokenUtil = new RefreshTokenUtil(refreshTokenRepo.Object);
            var useCase = new LoginUseCase(memberServiceMock.Object, jwtUtilMock, refreshTokenUtil);
            // act
            var result = await useCase.ExecuteAsync("UnitTest", "validAccount", "validPassword", 3);
            // assert
            Assert.Equal(result.Error.Code, "LoginUseCase.Exception");
        }

        // refresh token 產生失敗，返回系統有問題，請稍後重試
    }
}
