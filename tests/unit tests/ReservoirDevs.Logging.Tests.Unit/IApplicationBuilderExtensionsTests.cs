using System;
using FluentAssertions;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using ReservoirDevs.Logging.Extensions;
using ReservoirDevs.Logging.Middleware;
using Xunit;

namespace ReservoirDevs.Logging.Tests.Unit
{
    // ReSharper disable once InconsistentNaming
    public class IApplicationBuilderExtensionsTests
    {
        [Fact]
        public void LogRequestsAndResponses_AddsMiddleware()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<LogRequestsAndResponsesMiddleware>>();

            serviceProvider.Setup(provider => provider.GetService(typeof(ILogger<LogRequestsAndResponsesMiddleware>))).Returns(logger.Object);

            var applicationBuilder = new ApplicationBuilder(serviceProvider.Object);
            applicationBuilder.LogRequestsAndResponses();
            
            var sut = applicationBuilder.Build();

            sut.Method.DeclaringType.Should().BeAssignableTo<LogRequestsAndResponsesMiddleware>();
        }
    }
}