using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ReservoirDevs.Logging.Middleware;
using ReservoirDevs.Logging.Models;
using ReservoirDevs.Test.Helpers;
using Xunit;

namespace ReservoirDevs.Logging.Tests.Unit
{
    public class LogRequestsAndResponsesMiddlewareTests
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly Mock<ILogger<LogRequestsAndResponsesMiddleware>> _logger;
        private readonly HttpRequestModel _requestModel;
        
        private static MethodInfo MapHttpRequest => ReflectionHelper.GetStaticMethod<LogRequestsAndResponsesMiddleware>("MapHttpRequest");
        private static MethodInfo MapHttpResponse => ReflectionHelper.GetStaticMethod<LogRequestsAndResponsesMiddleware>("MapHttpResponse");

        public LogRequestsAndResponsesMiddlewareTests()
        {
            _requestDelegate = innerContext => Task.FromResult(0);
            _logger = new Mock<ILogger<LogRequestsAndResponsesMiddleware>>();
            _requestModel = new HttpRequestModel
            {
                Scheme = "A",
                Host = "B"
            };
        }
        
        [Fact]
        public void LogRequestsAndResponsesMiddleware_ThrowsException_WhenLoggerIsNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new LogRequestsAndResponsesMiddleware(_requestDelegate, null);

            act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("logger");
        }

        [Fact]
        public async Task Invoke_LogsRequestInformation_WhenRequestBodyNotPopulated()
        {
            var sut = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var httpContext = new DefaultHttpContext();

            await sut.Invoke(httpContext);

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Request Information")),
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
        public async Task Invoke_LogsRequestInformationWithBody_WhenRequestBodyPopulated()
        {
            var requestModel = new HttpRequestModel
            {
                Scheme = "A",
                Host = "B",
                Path = "/C",
                Body = "testing",
                Querystring = "?F=G"
            };

            requestModel.Headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Host", requestModel.Host),
                new KeyValuePair<string, string>("Content-Length", requestModel.Body.Length.ToString()),
                new KeyValuePair<string, string>("D", "E")
            };

            var responseModel = new HttpResponseModel
            {
                Body = requestModel.Body,
                Headers = requestModel.Headers
            };

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Scheme = requestModel.Scheme;
            httpContext.Request.Host = new HostString(requestModel.Host);
            httpContext.Request.Path = requestModel.Path;
            httpContext.Request.QueryString = new QueryString(requestModel.Querystring);
            httpContext.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestModel.Body));
            httpContext.Request.ContentLength = requestModel.Body.Length;
            httpContext.Request.Headers.Add(requestModel.Headers.Last().Key, requestModel.Headers.Last().Value);

            var sut = new LogRequestsAndResponsesMiddleware(innerContext =>
            {
                innerContext.Response.Headers.Add("Host", requestModel.Host);
                innerContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseModel.Body));
                innerContext.Response.ContentLength = responseModel.Body.Length;
                innerContext.Response.Headers.Add(responseModel.Headers.Last().Key, responseModel.Headers.Last().Value);
                return Task.FromResult(0);
            }, _logger.Object);

            await sut.Invoke(httpContext);

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Request Information") && LoggingMessageContains(item, requestModel.ToString())),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level.Equals(LogLevel.Information)),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((item, type) => LoggingMessageContains(item, "Http Response Information") && LoggingMessageContains(item, responseModel.ToString())),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Fact]
        public async Task MapHttpRequest_ShouldMapSchemeAndHost() {

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = _requestModel.Scheme;
            httpContext.Request.Host = new HostString(_requestModel.Host);

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpRequestModel>)MapHttpRequest.Invoke(middleware, new object[] { httpContext, httpContext.Request.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;

            sut.Scheme.Should().Be(_requestModel.Scheme);
            sut.Host.Should().Be(_requestModel.Host);
            sut.Path.Should().BeNullOrWhiteSpace();
            sut.Querystring.Should().BeNullOrWhiteSpace();
            sut.Headers.Count().Should().Be(httpContext.Request.Headers.Count);
            sut.Body.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public async Task MapHttpRequest_ShouldMapPath()
        {

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/C";

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpRequestModel>)MapHttpRequest.Invoke(middleware, new object[] { httpContext, httpContext.Request.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;
            
            sut.Path.Should().Be(httpContext.Request.Path);
        }

        [Fact]
        public async Task MapHttpRequest_ShouldMapHeaders()
        {

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("D", "E");

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpRequestModel>)MapHttpRequest.Invoke(middleware, new object[] { httpContext, httpContext.Request.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;

            sut.Headers.Any(item => item.Key.Equals("D") && item.Value.Equals("E")).Should().BeTrue();
        }

        [Fact]
        public async Task MapHttpRequest_ShouldMapQuerystring()
        {

            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?F=G");

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpRequestModel>)MapHttpRequest.Invoke(middleware, new object[] { httpContext, httpContext.Request.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;

            sut.Querystring.Should().Be(httpContext.Request.QueryString.Value);
        }

        [Fact]
        public async Task MapHttpRequest_ShouldMapBody()
        {
            const string content = "testing";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            httpContext.Request.ContentLength = content.Length;

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpRequestModel>)MapHttpRequest.Invoke(middleware, new object[] { httpContext, httpContext.Request.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;

            sut.Body.Should().Be(content);
        }

        [Fact]
        public async Task MapHttpResponse_ShouldMapHeaders()
        {

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Headers.Add("D", "E");

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpResponseModel>)MapHttpResponse.Invoke(middleware, new object[] { httpContext, httpContext.Response.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;

            sut.Headers.Any(item => item.Key.Equals("D") && item.Value.Equals("E")).Should().BeTrue();
        }

        [Fact]
        public async Task MapHttpResponse_ShouldMapBody()
        {
            const string content = "testing";

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            httpContext.Response.ContentLength = content.Length;

            var middleware = new LogRequestsAndResponsesMiddleware(_requestDelegate, _logger.Object);

            var result = (Task<HttpResponseModel>)MapHttpResponse.Invoke(middleware, new object[] { httpContext, httpContext.Response.Body });

            // ReSharper disable once PossibleNullReferenceException
            var sut = await result;

            sut.Body.Should().Be(content);
        }

        private static bool LoggingMessageContains(object subject, string expected)
        {
            return subject.ToString()?.Contains(expected) ?? false;
        }
    }
}
