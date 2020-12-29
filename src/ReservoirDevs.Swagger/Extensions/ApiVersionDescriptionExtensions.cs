using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReservoirDevs.Swagger.Extensions
{
    public static class ApiVersionDescriptionExtensions
    {
        public static void RegisterWithSwagger(this IEnumerable<ApiVersionDescription> descriptions, SwaggerGenOptions options, OpenApiInfo openApiInfoSettings)
        {
            foreach (var description in descriptions)
            {
                RegisterWithSwagger(description, options, openApiInfoSettings);
            }
        }

        public static void RegisterWithSwagger(this ApiVersionDescription description, SwaggerGenOptions options, OpenApiInfo openApiInfoSettings)
        {
            openApiInfoSettings.Version = description.ApiVersion.ToString();

            options.SwaggerDoc(description.GroupName, openApiInfoSettings);
        }
    }
}