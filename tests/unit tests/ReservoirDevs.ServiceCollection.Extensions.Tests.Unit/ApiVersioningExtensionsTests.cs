using System;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ReservoirDevs.ServiceCollection.Extensions.Tests.Unit
{
    public class ApiVersioningExtensionsTests
    {
        [Fact]
        public void AddApiVersioning_InjectsExpectedDependencies()
        {
            Environment.SetEnvironmentVariable("A:ReportApiVersions", "true");
            
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddEnvironmentVariables();
            
            var configuration = configurationBuilder.Build();

            var collection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            var countBefore = collection.Count;
            
            collection.AddApiVersioning(configuration.GetSection("A"));

            var countAfter = collection.Count;

            countAfter.Should().BeGreaterThan(countBefore);

            collection.Any(item => item.ServiceType == typeof(IApiVersionReader)).Should().BeTrue();
            collection.Any(item => item.ServiceType == typeof(IApiVersionSelector)).Should().BeTrue();
            collection.Any(item => item.ServiceType == typeof(IApiVersionParameterSource)).Should().BeTrue();
        }

        [Fact]
        public void AddVersionedApiExplorer_InjectsExpectedDependencies()
        {
            Environment.SetEnvironmentVariable("A:DefaultApiVersion", "1");

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();

            var collection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            var countBefore = collection.Count;

            collection.AddVersionedApiExplorer(configuration.GetSection("A"));

            var countAfter = collection.Count;

            countAfter.Should().BeGreaterThan(countBefore);

            collection.Any(item => item.ServiceType == typeof(IApiDescriptionGroupCollectionProvider)).Should().BeTrue();
            collection.Any(item => item.ServiceType == typeof(IApiDescriptionProvider)).Should().BeTrue();
            collection.Any(item => item.ServiceType == typeof(IApiVersionDescriptionProvider)).Should().BeTrue();
        }
    }
}