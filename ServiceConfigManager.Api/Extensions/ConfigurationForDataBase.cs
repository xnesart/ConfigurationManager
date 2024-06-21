using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServiceConfigManager.Core;
using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.Api.Extensions;

public static class ConfigurationForDataBase
{
    public static void ConfigureDataBase(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var connectionString = Environment.GetEnvironmentVariable("MypConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        var dataSource = dataSourceBuilder.Build();
        dataSourceBuilder.MapEnum<ServiceType>();
        services.AddDbContext<DataBaseContext>(
            options => options
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention());

    }

   
    public static async Task MigrateAndReloadPostgresTypesAsync(this IServiceProvider serviceProvider, CancellationToken token = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

        await dbContext.Database.MigrateAsync(token);

        if (dbContext.Database.GetDbConnection() is NpgsqlConnection npgsqlConnection)
        {
            await npgsqlConnection.OpenAsync(token);

            try
            {
                await npgsqlConnection.ReloadTypesAsync();
            }
            finally
            {
                await npgsqlConnection.CloseAsync();
            }
        }
    }
}