using Microsoft.EntityFrameworkCore;
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

    public async Task<Guid> AddConfigurationForService(ServiceConfigurationDto newConfiguration)
    {
        _logger.Information($"Сервисы: добавление конфигурации: добавляем новую конфигурацию в контекст");
        
        if (await CheckForExistingConfiguration(newConfiguration)) throw new InvalidOperationException("Такая конфигурация уже существует, используйте PUT endpoint");
        
        await _ctx.AddAsync(newConfiguration);
        await _ctx.SaveChangesAsync();

        _logger.Information($"Возвращаем айди конфигурации {newConfiguration.Id}");

        return newConfiguration.Id;
    }

    private async Task<bool> CheckForExistingConfiguration(ServiceConfigurationDto newConfiguration)
    {
        _logger.Information($"Проверяем, существует ли в базе такая конфигурация {newConfiguration.ServiceType}");
        var exists = await _ctx.ServiceConfiguration.AnyAsync(t => t.ServiceType == newConfiguration.ServiceType);
        
        return exists;
    }
}