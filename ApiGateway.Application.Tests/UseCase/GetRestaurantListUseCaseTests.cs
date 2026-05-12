using ApiGateway.Application.Abstractions.External;
using ApiGateway.Application.DTO.RestaurantServiceClient;
using ApiGateway.Application.UseCase;
using Moq;

namespace ApiGateway.Application.Tests.UseCase
{
    public class GetRestaurantListUseCaseTests
    {
        // 有符合 keyword 的資料，回傳非空列表
        [Fact]
        public async Task GetRestaurantList_ShouldReturnList_WhenKeywordMatches()
        {
            // arrange
            var clientMock = new Mock<IRestaurantServiceClient>();
            clientMock
                .Setup(m => m.GetRestaurantList(It.IsAny<string>()))
                .ReturnsAsync(new GetRestaurantListResponseDto
                {
                    RestaurantList = new List<RestaurantItemDto>
                    {
                        new() { RestaurantId = 1, Name = "壽司太郎" }
                    }
                });
            var useCase = new GetRestaurantListUseCase(clientMock.Object);

            // act
            var result = await useCase.ExecuteAsync("壽司");

            // assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!.RestaurantList);
            Assert.Equal(1, result.Value.RestaurantList[0].RestaurantId);
            Assert.Equal("壽司太郎", result.Value.RestaurantList[0].Name);
        }

        // 無符合 keyword 的資料，回傳空列表（仍為成功）
        [Fact]
        public async Task GetRestaurantList_ShouldReturnEmptyList_WhenNoMatch()
        {
            // arrange
            var clientMock = new Mock<IRestaurantServiceClient>();
            clientMock
                .Setup(m => m.GetRestaurantList(It.IsAny<string>()))
                .ReturnsAsync(new GetRestaurantListResponseDto { RestaurantList = new() });
            var useCase = new GetRestaurantListUseCase(clientMock.Object);

            // act
            var result = await useCase.ExecuteAsync("不存在的餐廳");

            // assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!.RestaurantList);
        }

        // Restaurant.API 拋出例外，回傳 Fail
        [Fact]
        public async Task GetRestaurantList_ShouldReturnFail_WhenServiceThrows()
        {
            // arrange
            var clientMock = new Mock<IRestaurantServiceClient>();
            clientMock
                .Setup(m => m.GetRestaurantList(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Connection refused"));
            var useCase = new GetRestaurantListUseCase(clientMock.Object);

            // act
            var result = await useCase.ExecuteAsync("壽司");

            // assert
            Assert.False(result.IsSuccess);
            Assert.Equal("GetRestaurantListUseCase.Exception", result.Error.Code);
        }
    }
}
