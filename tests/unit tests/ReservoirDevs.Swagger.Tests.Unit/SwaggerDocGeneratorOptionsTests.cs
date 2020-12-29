using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using ReservoirDevs.Swagger.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace ReservoirDevs.Swagger.Tests.Unit
{
    public class SwaggerDocGeneratorOptionsTests
    {
        [Fact]
        public void SwaggerDocGeneratorOptions_ThrowsException_WhenProviderIsNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action sut = () => new SwaggerDocGeneratorOptions(null, null, null);

            sut.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("provider");
        }

        [Fact]
        public void SwaggerDocGeneratorOptions_ThrowsException_WhenOpenApiInfoSettingsIsNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action sut = () => new SwaggerDocGeneratorOptions(new Mock<IApiVersionDescriptionProvider>().Object, null, null);

            sut.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("openApiInfoSettings");
        }

        [Fact]
        public void SwaggerDocGeneratorOptions_DoesNotThrowException_WhenAllParametersArePopulated()
        {
            var info = new OpenApiInfo();

            // ReSharper disable once ObjectCreationAsStatement
            Action sut = () => new SwaggerDocGeneratorOptions(new Mock<IApiVersionDescriptionProvider>().Object, Microsoft.Extensions.Options.Options.Create(info), null);

            sut.Should().NotThrow<ArgumentNullException>();
        }

        [Fact]
        public void SwaggerDocGeneratorOptions_RegistersDescriptors_WhenInputIsValid()
        {
            const string key = "A";
            var info = new OpenApiInfo();
            
            var provider = new Mock<IApiVersionDescriptionProvider>();
            var options = new SwaggerGenOptions();
            var swaggerDocs = new Mock<IDictionary<string, OpenApiInfo>>();

            var generatorOptions = new SwaggerGeneratorOptions {SwaggerDocs = swaggerDocs.Object};
            
            options.SwaggerGeneratorOptions = generatorOptions;
            provider.Setup(item => item.ApiVersionDescriptions).Returns(new List<ApiVersionDescription>
            {
                new ApiVersionDescription(new ApiVersion(1, 0), key, false)
            });

            var sut = new SwaggerDocGeneratorOptions(provider.Object, Microsoft.Extensions.Options.Options.Create(info), null);

            sut.Configure(options);

            swaggerDocs.Verify(
                item => item.Add(
                    It.Is<string>(value => value.Equals(key)),
                    It.Is<OpenApiInfo>(apiInfo => apiInfo.Equals(info))
                )
            );
        }

        [Fact]
        public void SwaggerDocGeneratorOptions_RegistersFilters_WhenInputIsValid()
        {
            const string key = "A";
            var info = new OpenApiInfo();

            var provider = new Mock<IApiVersionDescriptionProvider>();
            var options = new SwaggerGenOptions();
            var swaggerDocs = new Mock<IDictionary<string, OpenApiInfo>>();

            var generatorOptions = new SwaggerGeneratorOptions { SwaggerDocs = swaggerDocs.Object };

            options.SwaggerGeneratorOptions = generatorOptions;
            provider.Setup(item => item.ApiVersionDescriptions).Returns(new List<ApiVersionDescription>
            {
                new ApiVersionDescription(new ApiVersion(1, 0), key, false)
            });

            var filterCountBefore = options.OperationFilterDescriptors.Count;
            
            var sut = new SwaggerDocGeneratorOptions(provider.Object, Microsoft.Extensions.Options.Options.Create(info), new List<IOperationFilter>{new Mock<IOperationFilter>().Object});

            sut.Configure(options);
            
            var filterCountAfter = options.OperationFilterDescriptors.Count;

            filterCountAfter.Should().BeGreaterThan(filterCountBefore);
        }
    }
}
