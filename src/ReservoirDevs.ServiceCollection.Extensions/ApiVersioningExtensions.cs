using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ReservoirDevs.ServiceCollection.Extensions
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection serviceCollection, IConfigurationSection configurationSection)
        {
            return serviceCollection.AddApiVersioning(configurationSection.Bind);
        }

        public static IServiceCollection AddVersionedApiExplorer(this IServiceCollection serviceCollection, IConfigurationSection configurationSection)
        {
            return serviceCollection.AddVersionedApiExplorer(configurationSection.Bind);
        }
    }
}
