using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using ReservoirDevs.Correlation.Extensions;
using Xunit;

namespace ReservoirDevs.Correlation.Tests.Unit
{
    // ReSharper disable once InconsistentNaming
    public class IApplicationBuilderExtensionsTests
    {
        [Fact]
        public void RequireCorrelationId_TriggersUse()
        {
            var sut = new Mock<IApplicationBuilder>();

            sut.Object.RequireCorrelationId();

            sut.Verify(item => item.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }
    }
}