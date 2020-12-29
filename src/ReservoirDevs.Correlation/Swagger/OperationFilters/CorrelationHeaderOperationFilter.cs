using System;
using Microsoft.OpenApi.Models;
using ReservoirDevs.Correlation.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReservoirDevs.Correlation.Swagger.OperationFilters
{
    public class CorrelationHeaderOperationFilter : IOperationFilter
    {
        private readonly string _headerKey;
        
        public CorrelationHeaderOperationFilter(CorrelationHeader correlationHeader)
        {
            if (string.IsNullOrWhiteSpace(correlationHeader))
            {
                throw new ArgumentNullException(nameof(correlationHeader));
            }
            _headerKey = correlationHeader;
        }
        
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = _headerKey,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "String"
                }
            });
        }
    }
}