using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ReservoirDevs.Correlation.Middleware;
using Xunit;

namespace ReservoirDevs.Correlation.Tests.Unit
{
    public class CorrelationIdMiddlewareTests
    {
        private readonly Mock<ILogger<CorrelationIdMiddleware>> _logger;

        public CorrelationIdMiddlewareTests()
        {
            _logger = new Mock<ILogger<CorrelationIdMiddleware>>();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void CorrelationIdMiddleware_ThrowsArgumentNullException_WhenHeaderValueEmptyOrWhitespace(string value)
        {
            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));

            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new CorrelationIdMiddleware(requestDelegate, value, null);

            act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Fact]
        public void CorrelationIdMiddleware_ThrowsArgumentNullException_WhenLoggerNull()
        {
            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));

            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new CorrelationIdMiddleware(requestDelegate, "A", null);

            act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("logger");
        }

        [Fact]
        public async Task Invoke_ReturnsValidationError_WhenHeaderIsMissing()
        {
            const string headerKey = "A";
            
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var requestDelegate = new Mock<RequestDelegate>();

            var sut = new CorrelationIdMiddleware(requestDelegate.Object, headerKey, _logger.Object);

            await sut.Invoke(httpContext);
            
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            var response = Deserialize<ValidationProblemDetails>(httpContext.Response.Body);

            response.Should().NotBeNull();

            response.Errors.Count.Should().BeGreaterThan(0);
            response.Errors.Keys.Contains(headerKey).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public async Task Invoke_ReturnsValidationError_WhenHeaderHasNoValue(string value)
        {
            const string headerKey = "A";

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            httpContext.Request.Headers.Add(headerKey, value);

            var requestDelegate = new Mock<RequestDelegate>();

            var sut = new CorrelationIdMiddleware(requestDelegate.Object, headerKey, _logger.Object);

            await sut.Invoke(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            var response = Deserialize<ValidationProblemDetails>(httpContext.Response.Body);

            response.Should().NotBeNull();

            response.Errors.Count.Should().BeGreaterThan(0);
            response.Errors.Keys.Contains(headerKey).Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_CallsNextMiddleware_WhenHeaderIsPresentAndValid()
        {
            const string headerKey = "A";

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            httpContext.Request.Headers.Add(headerKey, "B");

            var requestDelegate = new Mock<RequestDelegate>();

            var sut = new CorrelationIdMiddleware(requestDelegate.Object, headerKey, _logger.Object);

            await sut.Invoke(httpContext);

            requestDelegate.Verify(action => action(httpContext), Times.Once);
        }

        private static T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                using (var textReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(textReader);
                }
            }
        }
    }
}
