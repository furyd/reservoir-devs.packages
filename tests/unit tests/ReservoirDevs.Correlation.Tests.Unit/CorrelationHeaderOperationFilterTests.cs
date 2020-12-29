using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using ReservoirDevs.Correlation.Swagger.OperationFilters;
using Xunit;

namespace ReservoirDevs.Correlation.Tests.Unit
{
    public class CorrelationHeaderOperationFilterTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void CorrelationHeaderOperationFilter_ThrowsArgumentNullException_WhenHeaderValueEmptyOrWhitespace(string value)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new CorrelationHeaderOperationFilter(value);

            act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("correlationHeader");
        }

        [Fact]
        public void Apply_AddsParameterWithExpectedValues()
        {
            const string headerKey = "A";
            
            var sut = new CorrelationHeaderOperationFilter(headerKey);

            var openApiOperation = new OpenApiOperation();

            var parametersCountBefore = openApiOperation.Parameters.Count;

            parametersCountBefore.Should().Be(0);
            
            sut.Apply(openApiOperation, null);

            var parametersCountAfter = openApiOperation.Parameters.Count;

            parametersCountAfter.Should().Be(1);

            openApiOperation.Parameters.First().Name.Should().Be(headerKey);
            openApiOperation.Parameters.First().In.Should().Be(ParameterLocation.Header);
            openApiOperation.Parameters.First().Required.Should().BeTrue();
            openApiOperation.Parameters.First().Schema.Type.Should().Be("String");
        }
    }
}