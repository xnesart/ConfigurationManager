using ConfigurationManager.Bll.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationManager.Bll;

public static class ConfigurationForServices
{
    public static void ConfigureBllServices(this IServiceCollection services)
    {
        // services.AddScoped<IValidator<CreateUserRequest>, UserRequestValidator>();
        // services.AddScoped<IValidator<UpdateUserRequest>, UserUpdateValidator>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        // services.AddScoped<IDevicesService, DevicesService>();
    }
}