using Microsoft.Extensions.DependencyInjection;
using ServiceConfigManager.Bll.Services;

namespace ServiceConfigManager.Bll;

public static class ConfigurationForServices
{
    public static void ConfigureBllServices(this IServiceCollection services)
    {
        services.AddScoped<IConfigurationService, ConfigurationService>();
    }
}