using Microsoft.EntityFrameworkCore;
using ServiceConfigManager.Core;

namespace ServiceConfigManager.Api.Extensions;

public static class ConfigurationForDataBase
{
    public static void ConfigureDataBase(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddDbContext<DataBaseContext>(
            options => options
                .UseNpgsql(configurationManager.GetConnectionString("MypConnection"))
                .UseSnakeCaseNamingConvention());
    }
}