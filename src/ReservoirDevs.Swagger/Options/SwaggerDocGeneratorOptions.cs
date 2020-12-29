using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ReservoirDevs.Swagger.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReservoirDevs.Swagger.Options
{
    public class SwaggerDocGeneratorOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly OpenApiInfo _openApiInfoSettings;
        private readonly IEnumerable<IOperationFilter> _operationFilters;

        public SwaggerDocGeneratorOptions(IApiVersionDescriptionProvider provider, IOptions<OpenApiInfo> openApiInfoSettings, IEnumerable<IOperationFilter> operationFilters)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _openApiInfoSettings = openApiInfoSettings?.Value ?? throw new ArgumentNullException(nameof(openApiInfoSettings));
            _operationFilters = operationFilters;
        }

        public void Configure(SwaggerGenOptions options)
        {
            _provider.ApiVersionDescriptions.RegisterWithSwagger(options, _openApiInfoSettings);
            ConfigureFilters(options);
        }

        private void ConfigureFilters(SwaggerGenOptions options)
        {
            if (_operationFilters == null || !_operationFilters.Any())
            {
                return;
            }
            
            foreach (var operationFilters in _operationFilters)
            {
                options.OperationFilterDescriptors.Add(operationFilters.ToFilterDescriptor());
            }
        }
    }
}