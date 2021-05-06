using AutoMapper;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.Common;
using MyHealth.Fitbit.Activity.Functions;
using MyHealth.Fitbit.Activity.Models;
using MyHealth.Fitbit.Activity.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.Fitbit.Activity.UnitTests.FunctionTests
{
    public class GetActivitySummaryByDateShould
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IFitbitApiService> _mockFitbitApiService;
        private Mock<IMapper> _mockMapper;
        private Mock<IServiceBusHelpers> _mockServiceBusHelpers;
        private Mock<ILogger> _mockLogger;
        private TimerInfo _testTimerInfo;

        private GetActivitySummaryByDate _func;

        public GetActivitySummaryByDateShould()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockFitbitApiService = new Mock<IFitbitApiService>();
            _mockMapper = new Mock<IMapper>();
            _mockServiceBusHelpers = new Mock<IServiceBusHelpers>();
            _mockLogger = new Mock<ILogger>();
            _testTimerInfo = default(TimerInfo);

            _func = new GetActivitySummaryByDate(
                _mockConfiguration.Object,
                _mockFitbitApiService.Object,
                _mockMapper.Object,
                _mockServiceBusHelpers.Object);
        }

        [Fact]
        public async Task RetrieveSleepLogResponseAndSendMappedObjectToSleepTopic()
        {
            // Arrange
            _mockFitbitApiService.Setup(x => x.GetActivityResponseObject(It.IsAny<string>())).ReturnsAsync(It.IsAny<ActivityResponseObject>());
            _mockMapper.Setup(x => x.Map(It.IsAny<ActivityResponseObject>(), It.IsAny<mdl.Activity>())).Verifiable();
            _mockServiceBusHelpers.Setup(x => x.SendMessageToTopic(It.IsAny<string>(), It.IsAny<mdl.Activity>())).Returns(Task.CompletedTask);

            // Act
            Func<Task> getDailySleepAction = async () => await _func.Run(_testTimerInfo, _mockLogger.Object);

            // Assert
            await getDailySleepAction.Should().NotThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToTopic(It.IsAny<string>(), It.IsAny<mdl.Activity>()), Times.Once);
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ThrowAndCatchExceptionWhenFitApiServiceThrowsException()
        {
            // Arrange
            _mockFitbitApiService.Setup(x => x.GetActivityResponseObject(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> getDailySleepAction = async () => await _func.Run(_testTimerInfo, _mockLogger.Object);

            // Assert
            await getDailySleepAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToTopic(It.IsAny<string>(), It.IsAny<mdl.Activity>()), Times.Never);
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task ThrowAndCatchExceptionWhenMapperThrowsException()
        {
            // Arrange
            _mockFitbitApiService.Setup(x => x.GetActivityResponseObject(It.IsAny<string>())).ReturnsAsync(It.IsAny<ActivityResponseObject>());
            _mockMapper.Setup(x => x.Map(It.IsAny<ActivityResponseObject>(), It.IsAny<mdl.Activity>())).Throws(new Exception());

            // Act
            Func<Task> getDailySleepAction = async () => await _func.Run(_testTimerInfo, _mockLogger.Object);

            // Assert
            await getDailySleepAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToTopic(It.IsAny<string>(), It.IsAny<mdl.Activity>()), Times.Never);
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task ThrowAndCatchExceptionWhenSendMessageToTopicThrowsException()
        {
            // Arrange
            _mockFitbitApiService.Setup(x => x.GetActivityResponseObject(It.IsAny<string>())).ReturnsAsync(It.IsAny<ActivityResponseObject>());
            _mockMapper.Setup(x => x.Map(It.IsAny<ActivityResponseObject>(), It.IsAny<mdl.Sleep>())).Verifiable();
            _mockServiceBusHelpers.Setup(x => x.SendMessageToTopic(It.IsAny<string>(), It.IsAny<mdl.Activity>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> getDailySleepAction = async () => await _func.Run(_testTimerInfo, _mockLogger.Object);

            // Assert
            await getDailySleepAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
