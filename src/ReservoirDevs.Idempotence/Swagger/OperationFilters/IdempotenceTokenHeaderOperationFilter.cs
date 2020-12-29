using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ReservoirDevs.Idempotence.Filters;
using ReservoirDevs.Idempotence.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using AccessViolationException = System.AccessViolationException;

namespace ReservoirDevs.Idempotence.Swagger.OperationFilters
{
    public class IdempotenceTokenHeaderOperationFilter : IOperationFilter
    {
        private readonly string _headerKey;

        public IdempotenceTokenHeaderOperationFilter(IdempotenceHeader idempotenceHeader)
        {
            if (string.IsNullOrWhiteSpace(idempotenceHeader))
            {
                throw new ArgumentNullException(nameof(idempotenceHeader));
            }
            
            _headerKey = idempotenceHeader;
        }
        
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.TryGetMethodInfo(out var methodInfo))
            {
                throw new AccessViolationException("Unable to get method info");
            }

            if (!(methodInfo.GetCustomAttributes(false).FirstOrDefault(item => item.GetType() == typeof(ServiceFilterAttribute)) is ServiceFilterAttribute serviceFilterAttribute))
            {
                return;
            }

            if (serviceFilterAttribute.ServiceType != typeof(IdempotenceFilter))
            {
                return;
            }

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