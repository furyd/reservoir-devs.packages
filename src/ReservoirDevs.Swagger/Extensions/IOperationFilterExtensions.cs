using System;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReservoirDevs.Swagger.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IOperationFilterExtensions
    {
        public static FilterDescriptor ToFilterDescriptor(this IOperationFilter operationFilter) => new FilterDescriptor
        {
            Type = operationFilter.GetType(),
            Arguments = Array.Empty<object>()
        };
    }
}