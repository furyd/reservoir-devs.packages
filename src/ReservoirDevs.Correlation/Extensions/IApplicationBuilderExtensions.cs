using Microsoft.AspNetCore.Builder;
using ReservoirDevs.Correlation.Middleware;

namespace ReservoirDevs.Correlation.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder RequireCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}