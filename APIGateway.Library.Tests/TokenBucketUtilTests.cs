using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.RateLimit;
using ApiGateway.Infrastructure.RateLimit;
using Moq;
using Microsoft.Extensions.Time.Testing;



namespace APIGateway.Infrastructure.Tests
{ 

    public class TokenBucketUtilTests
    {
        #region 測試用fake
        public class FakeGetTokenBucketInfoRepository : IGetTokenBucketInfo
        {
            private int _bucketAmount;
            private DateTime _lastRefillTime;

            public Task<TokenBucketInfo> GetTokenBucketInfoAsync(string apiKey, CancellationToken ct) => Task.FromResult(new TokenBucketInfo { token = _bucketAmount, lastRefillTime = _lastRefillTime });

            public Task UpdateBucketInfoAsync(TokenBucketInfo tokenBucketInfo)
            {
                _bucketAmount = tokenBucketInfo.token;
                _lastRefillTime = tokenBucketInfo.lastRefillTime;
                return Task.CompletedTask;
            }

            public FakeGetTokenBucketInfoRepository(int bucketAmount, TimeProvider timeProvider)
            {
                _bucketAmount = bucketAmount;
                _lastRefillTime = timeProvider.GetUtcNow().UtcDateTime;
            }
        }
        #endregion

        [Fact]
        public async Task CheckTokenBucketAmount_ShouldGetAmount()
        {
            // arrange
            var mockRepo = new Mock<IGetTokenBucketInfo>();
            mockRepo.Setup(r => r.GetTokenBucketInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TokenBucketInfo { token = 20, lastRefillTime = DateTime.UtcNow.AddMinutes(-5) });

            var util = new TokenBucketUtil(mockRepo.Object, TimeProvider.System);

            // act
            var result = await util.CheckTokenBucketAmount();

            // assert
            mockRepo.Verify(m => m.GetTokenBucketInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Moq.Times.Once);
        }

        [Fact]
        public async Task CheckTokenBucketAmount_BucketEmpty_ShouldReturnFalse()
        {
            // arrange
            var mockRepo = new Mock<IGetTokenBucketInfo>();
            mockRepo.Setup(r => r.GetTokenBucketInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TokenBucketInfo { token = 0, lastRefillTime = DateTime.UtcNow });
            var util = new TokenBucketUtil(mockRepo.Object, TimeProvider.System);

            // act
            var result = await util.CheckTokenBucketAmount();

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckTokenBucketAmount_BucketHasToken_ShouldReturnTrue()
        {
            // arrange
            var mockRepo = new Mock<IGetTokenBucketInfo>();

            mockRepo.Setup(r => r.GetTokenBucketInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TokenBucketInfo { token = 20, lastRefillTime = DateTime.UtcNow.AddMinutes(-5) });
            var util = new TokenBucketUtil(mockRepo.Object, TimeProvider.System);

            // act
            var result = await util.CheckTokenBucketAmount();

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckTokenBucketAmount_BucketRefill_ShouldRefillCorrectly()
        {
            // arrange : 先有1個token
            var fakeTimeProvider = new FakeTimeProvider();
            var fakeRepo = new FakeGetTokenBucketInfoRepository(1, fakeTimeProvider); // 初始有1個token

            var util = new TokenBucketUtil(fakeRepo, fakeTimeProvider);

            // act & Assert
            Assert.True(await util.CheckTokenBucketAmount()); // 第一次檢查，應該有token
            Assert.False(await util.CheckTokenBucketAmount()); // 失敗

            fakeTimeProvider.Advance(new TimeSpan(0,0,2)); // 時間前進兩秒

            Assert.True(await util.CheckTokenBucketAmount()); 
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.False(await util.CheckTokenBucketAmount());

        }

        public async void CheckTokenBucketAmount_BucketRefill_ShouldNotOverCapacity()
        {
            // arrange
            var fakeTimeProvider = new FakeTimeProvider();
            var fakeRepo = new FakeGetTokenBucketInfoRepository(9, fakeTimeProvider); // 初始有1個token

            var util = new TokenBucketUtil(fakeRepo, fakeTimeProvider);

            //act & assert

            fakeTimeProvider.Advance(new TimeSpan(0, 0, 2)); // 時間前進兩秒

            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.True(await util.CheckTokenBucketAmount());
            Assert.False(await util.CheckTokenBucketAmount());
        }
    }
}
