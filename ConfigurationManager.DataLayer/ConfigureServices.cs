using ConfigurationManager.DataLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationManager.DataLayer;

public static class ConfigureServices
{
    public static void ConfigureDalServices(this IServiceCollection services)
    {
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        // services.AddScoped<IDevicesRepository, DevicesRepository>();
    }
}