using Microsoft.AspNetCore.Builder;
using ReservoirDevs.Logging.Middleware;

namespace ReservoirDevs.Logging.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder LogRequestsAndResponses(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogRequestsAndResponsesMiddleware>();
        }
    }
}