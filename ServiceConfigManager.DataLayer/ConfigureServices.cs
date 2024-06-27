using Microsoft.Extensions.DependencyInjection;
using ServiceConfigManager.DataLayer.Repositories;

namespace ServiceConfigManager.DataLayer;

public static class ConfigureServices
{
    public static void ConfigureDalServices(this IServiceCollection services)
    {
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
    }
}