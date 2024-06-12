using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServiceConfigManager.Bll.Services;

namespace ServiceConfigManager.Bll;

public static class ConfigurationForServices
{
    public static void ConfigureBllServices(this IServiceCollection services)
    {
        // services.AddScoped<IValidator<CreateUserRequest>, UserRequestValidator>();
        // services.AddScoped<IValidator<UpdateUserRequest>, UserUpdateValidator>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
    }
}