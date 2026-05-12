using ApiGateway.Application.Abstractions;
using ApiGateway.Application.UseCase;
using APP.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sushi.All.Infrastructure.Result;

namespace APP.API.Tests
{
    public class RestaurantTests
    {
        // UseCase 成功回傳列表，Controller 應回傳 HTTP 200
        [Fact]
        public async Task List_ShouldReturn200_WithRestaurantList()
        {
            // arrange
            var useCaseMock = new Mock<IGetRestaurantListUseCase>();
            var expected = new RestaurantListRes(new List<RestaurantItemRes>
            {
                new(1, "壽司太郎")
            });
            useCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<RestaurantListRes>.Ok(expected));

            var controller = new RestaurantController(useCaseMock.Object);

            // act
            var actionResult = await controller.List("壽司");

            // assert
            var result = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var data = Assert.IsType<RestaurantListRes>(result.Value);
            Assert.Single(data.RestaurantList);
            Assert.Equal(1, data.RestaurantList[0].RestaurantId);
        }

        // UseCase 回傳 Fail，Controller 應回傳 HTTP 400
        [Fact]
        public async Task List_ShouldReturn400_WhenUseCaseFails()
        {
            // arrange
            var useCaseMock = new Mock<IGetRestaurantListUseCase>();
            useCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<RestaurantListRes>.Fail(
                    new Error("Restaurant.ServiceUnavailable", "Service error", ErrorType.Unexpected)));

            var controller = new RestaurantController(useCaseMock.Object);

            // act
            var actionResult = await controller.List("");

            // assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
    }
}
