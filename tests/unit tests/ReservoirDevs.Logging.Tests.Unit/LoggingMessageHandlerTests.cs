using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using ReservoirDevs.Logging.MessageHandlers;
using ReservoirDevs.Test.Helpers;
using Xunit;

namespace ReservoirDevs.Logging.Tests.Unit
{
    public class LoggingMessageHandlerTests
    {
        private readonly Mock<ILogger<LoggingMessageHandler>> _logger;

        public LoggingMessageHandlerTests()
        {
            _logger = new Mock<ILogger<LoggingMessageHandler>>();
        }

        [Fact]
        public async Task SendAsync_LogsRequestAndResponse()
        {
            var requestMessage = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri("http://a.b")};
            requestMessage.Headers.Add("A", "B");

            var sut = new LoggingMessageHandler(_logger.Object);

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("test", Encoding.UTF32, "text/text")
            };

            sut.InnerHandler = new MockHandler(responseMessage);

            var method = ReflectionHelper.GetInstanceMethod<LoggingMessageHandler>("SendAsync");

            var resultTask = (Task<HttpResponseMessage>)method.Invoke(sut, new object[]{ requestMessage, CancellationToken.None});

            if (resultTask != null)
            {
                var _ = await resultTask;
            }

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Request Information")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Response Information") && LoggingMessageContains(item, "test")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Fact]
        public async Task SendAsync_LogsRequestAndResponse_WhenRequestContentIsEmptyString()
        {
            var requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://a.b"), Content = new StringContent(string.Empty, Encoding.UTF32, "text/text") };

            var sut = new LoggingMessageHandler(_logger.Object);

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Headers.Add("A", "B");

            sut.InnerHandler = new MockHandler(responseMessage);

            var method = ReflectionHelper.GetInstanceMethod<LoggingMessageHandler>("SendAsync");

            var resultTask = (Task<HttpResponseMessage>)method.Invoke(sut, new object[] { requestMessage, CancellationToken.None });

            if (resultTask != null)
            {
                var _ = await resultTask;
            }

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Request Information") && LoggingMessageContains(item, "\"Body\":\"\"")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Response Information")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Fact]
        public async Task SendAsync_LogsRequestAndResponse_WhenResponseContentIsEmptyString()
        {
            var requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://a.b") };
            
            var sut = new LoggingMessageHandler(_logger.Object);

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty, Encoding.UTF32, "text/text")
            };
            responseMessage.Headers.Add("A", "B");

            sut.InnerHandler = new MockHandler(responseMessage);

            var method = ReflectionHelper.GetInstanceMethod<LoggingMessageHandler>("SendAsync");

            var resultTask = (Task<HttpResponseMessage>)method.Invoke(sut, new object[] { requestMessage, CancellationToken.None });

            if (resultTask != null)
            {
                var _ = await resultTask;
            }

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Request Information")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Response Information") && LoggingMessageContains(item, "\"Body\":\"\"")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Fact]
        public async Task SendAsync_LogsRequestAndResponse_WhenResponseContentIsNull()
        {
            var requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://a.b") };

            var sut = new LoggingMessageHandler(_logger.Object);

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Headers.Add("A", "B");

            sut.InnerHandler = new MockHandler(responseMessage);

            var method = ReflectionHelper.GetInstanceMethod<LoggingMessageHandler>("SendAsync");

            var resultTask = (Task<HttpResponseMessage>)method.Invoke(sut, new object[] { requestMessage, CancellationToken.None });

            if (resultTask != null)
            {
                var _ = await resultTask;
            }

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Request Information")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Response Information") && LoggingMessageContains(item, "\"Body\":null")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        private static bool LoggingMessageContains(object subject, string expected)
        {
            return subject.ToString()?.Contains(expected) ?? false;
        }
    }
}