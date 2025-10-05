using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Authentication;
using ApiGateway.Infrastructure.Authentication;
using Moq;

namespace APIGateway.Infrastructure.Tests
{
    public class SessionIdUtilTests
    {
        [Fact]
        public async void GenerateSessionId_ShouldReturnSessionId()
        {
            // arrange
            var mockRepo = new Mock<ISessionIdRestore>();
            var util = new SessionIdUtil(mockRepo.Object);

            // act
            var sessionId = await util.GenerateSessionId(0);

            // assert
            Assert.NotEqual(Guid.Empty, sessionId);
        }

        [Fact]
        public async void GenerateSessionID_ShouldRestore()
        {
            // arrange
            var mockRepo = new Mock<ISessionIdRestore>();
            var util = new SessionIdUtil(mockRepo.Object);

            // act
            var sessionId = await util.GenerateSessionId(0);

            // assert
            mockRepo.Verify(m => m.SaveSessionId(It.IsAny<Guid>(),It.IsAny<int>(), It.IsAny<CancellationToken>()),Times.Once);

        }

        [Fact]
        public async void ValidateSessionId_ShouldSearchFromDb()
        {
            // arrange
            var mockRepo = new Mock<ISessionIdRestore>();
            var util = new SessionIdUtil(mockRepo.Object);

            // act
            var result = await util.ValidateSessionId(Guid.NewGuid());

            // assert
            mockRepo.Verify(m => m.GetSessionId(It.IsAny<Guid>()), Times.Once);

        }
    }
}
