using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.UseCase;
using APP.API.Controllers;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Sushi.All.Infrastructure.Result;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace APP.API.Tests
{
    public class UserTests
    {
        [Fact]
        public void Login_Success_ReturnTokens()
        {
            // Arrange, Act
            var moqUseCase = new Moq.Mock<ILoginUseCase>();
            moqUseCase
                .Setup(x => x.ExecuteAsync("aaaaaaa", "111454535354",It.IsAny<string>(),It.IsAny<int>()))
                .ReturnsAsync(Result<LoginRes>.Ok(new LoginRes("access_token_example", "refresh_token_example")));

            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
                ["GatewayName"]="APP",
                ["TokenLifeHour"]="7"
            }).Build();
            var postResult = new UserController(moqUseCase.Object, config).Login(new Models.LoginReq("aaaaaaa", "111454535354")).Result;

            // Assert
            var result = Assert.IsType<OkObjectResult>(postResult);

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var data = Assert.IsType<LoginRes>(result.Value);
            Assert.NotNull(data.RefreshToken);
            Assert.NotNull(data.AccessToken);
        }

        
        [Fact]
        public void Login_InvalidParameters_ReturnBadRequest()
        {
            // 帳號密碼是否有值，且帳號至少6碼，密碼至少8碼
            // Arrange, Act
            var moqUseCase = new Moq.Mock<ILoginUseCase>();
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GatewayName"] = "APP",
                ["TokenLifeHour"] = "7"
            }).Build();
            var postResult = new UserController(moqUseCase.Object, config).Login(new Models.LoginReq("aaa", "111")).Result;
            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(postResult);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        // 輸入錯誤密碼會回傳失敗
        public void Login_InvalidPassword_ReturnBadRequest()
        {
            // Arrange
            var moqLoginUseCase = new Moq.Mock<ILoginUseCase>();
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GatewayName"] = "APP",
                ["TokenLifeHour"] = "7"
            }).Build();
            // Act
            var postResult = new UserController(moqLoginUseCase.Object,config).Login(new Models.LoginReq("aaaaaa", "11111111")).Result;
            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(postResult);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void LogIn_GetTokenFromApplication()
        {
            // arrange
            var moqLoginUseCase = new Moq.Mock<ILoginUseCase>();
            moqLoginUseCase
                .Setup(x => x.ExecuteAsync("aaaaaaa", "111454535354",It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Result<LoginRes>.Ok(new LoginRes("access_token_example", "refresh_token_example")));
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GatewayName"] = "APP",
                ["TokenLifeHour"] = "7"
            }).Build();
            // act
            var postResult = new UserController(moqLoginUseCase.Object, config).Login(new Models.LoginReq("aaaaaaa", "111454535354")).Result;

            // Assert
            var result = Assert.IsType<OkObjectResult>(postResult);

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            Assert.True(moqLoginUseCase.Invocations.Count > 0);
        }
    }
}
