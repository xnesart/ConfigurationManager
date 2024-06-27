using ServiceConfigManager.Api.Configuration.Filters;
using ServiceConfigManager.Bll.Services;

namespace ServiceConfigManager.Api.Extensions;

public static class ConfigurationForServices
{
    public static void ConfigureApiServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.ConfigureDataBase(configurationManager);
        services.AddTransient<IIpBlockingService, IpBlockingService>();
        services.AddScoped<IpBlockActionFilter>();
    }
}