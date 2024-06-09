using Serilog;
using ServiceConfigManager.Core;
using ServiceConfigManager.Core.DTOs;

namespace ServiceConfigManager.DataLayer.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    protected readonly DataBaseContext _ctx;
    private readonly ILogger _logger = Log.ForContext<ConfigurationRepository>();
    public ConfigurationRepository(DataBaseContext ctx)
    {
        _ctx = ctx;
    }

    public Guid AddConfigurationForService(ServiceConfigurationDto newConfiguration)
    {
        _logger.Information($"Сервисы: добавление конфигурации: добавляем новую конфигурацию в контекст");

        _ctx.Add(newConfiguration);
        _ctx.SaveChanges();
        
        _logger.Information($"Возвращаем айди конфигурации {newConfiguration.Id}");

        return newConfiguration.Id;
    }
}