using System;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.OpenApi.Models;
using ReservoirDevs.Idempotence.Swagger.OperationFilters;
using ReservoirDevs.Idempotence.Tests.Unit.SupportingClasses;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace ReservoirDevs.Idempotence.Tests.Unit
{
    public class IdempotenceTokenHeaderOperationFilterTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void IdempotenceTokenHeaderOperationFilter_ThrowsArgumentNullException_WhenHeaderValueEmptyOrWhitespace(string value)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new IdempotenceTokenHeaderOperationFilter(value);

            act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("idempotenceHeader");
        }

        [Fact]
        public void Apply_DoesNotAddParameter_WhenNoServiceFilterAttributePresent1()
        {
            const string headerKey = "A";

            var filter = new IdempotenceTokenHeaderOperationFilter(headerKey);

            var openApiOperation = new OpenApiOperation();

            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new CompiledPageActionDescriptor()
            };

            var context = new OperationFilterContext(apiDescription, null, null, null);

            Action sut = () => filter.Apply(openApiOperation, context);

            sut.Should().ThrowExactly<AccessViolationException>();
        }

        [Fact]
        public void Apply_DoesNotAddParameter_WhenNoServiceFilterAttributePresent()
        {
            const string headerKey = "A";

            var sut = new IdempotenceTokenHeaderOperationFilter(headerKey);

            var openApiOperation = new OpenApiOperation();

            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor { MethodInfo = typeof(TestController).GetMethod("TestMethodNoServiceFilter") }
            };

            var context = new OperationFilterContext(apiDescription, null, null, null);

            var parametersCountBefore = openApiOperation.Parameters.Count;

            parametersCountBefore.Should().Be(0);

            sut.Apply(openApiOperation, context);

            var parametersCountAfter = openApiOperation.Parameters.Count;

            parametersCountAfter.Should().Be(0);
        }

        [Fact]
        public void Apply_DoesNotAddParameter_WhenServiceFilterAttributePresentButNoneThatWrapIdempotenceFilter()
        {
            const string headerKey = "A";

            var sut = new IdempotenceTokenHeaderOperationFilter(headerKey);

            var openApiOperation = new OpenApiOperation();

            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor { MethodInfo = typeof(TestController).GetMethod("TestMethodNoIdempotenceFilter") }
            };

            var context = new OperationFilterContext(apiDescription, null, null, null);

            var parametersCountBefore = openApiOperation.Parameters.Count;

            parametersCountBefore.Should().Be(0);

            sut.Apply(openApiOperation, context);

            var parametersCountAfter = openApiOperation.Parameters.Count;

            parametersCountAfter.Should().Be(0);
        }

        [Fact]
        public void Apply_AddsParameterWithExpectedValues()
        {
            const string headerKey = "A";

            var sut = new IdempotenceTokenHeaderOperationFilter(headerKey);

            var openApiOperation = new OpenApiOperation();

            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor { MethodInfo = typeof(TestController).GetMethod("TestMethod") }
            };

            var context = new OperationFilterContext(apiDescription, null, null, null);

            var parametersCountBefore = openApiOperation.Parameters.Count;

            parametersCountBefore.Should().Be(0);

            sut.Apply(openApiOperation, context);

            var parametersCountAfter = openApiOperation.Parameters.Count;

            parametersCountAfter.Should().Be(1);

            openApiOperation.Parameters.First().Name.Should().Be(headerKey);
            openApiOperation.Parameters.First().In.Should().Be(ParameterLocation.Header);
            openApiOperation.Parameters.First().Required.Should().BeTrue();
            openApiOperation.Parameters.First().Schema.Type.Should().Be("String");
        }
    }
}