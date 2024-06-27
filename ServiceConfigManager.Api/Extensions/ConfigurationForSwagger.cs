using Microsoft.OpenApi.Models;

namespace ServiceConfigManager.Api.Extensions;

public static class ConfigurationForSwagger
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ConfigurationManager", Version = "v1" });
        });
    }
}