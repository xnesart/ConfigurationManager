using Microsoft.EntityFrameworkCore;
using Serilog;
using ServiceConfigManager.Core;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.DataLayer.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    protected readonly DataBaseContext _ctx;
    private readonly ILogger _logger = Log.ForContext<ConfigurationRepository>();

    public ConfigurationRepository(DataBaseContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Dictionary<string, string>> AddConfigurationForService(ServiceConfigurationDto newConfiguration)
    {
        _logger.Information($"Сервисы: добавление конфигурации: добавляем новую конфигурацию в контекст");
        await _ctx.AddAsync(newConfiguration);
        await _ctx.SaveChangesAsync();

        var configuration = await GetConfiguration(newConfiguration.ServiceType);
        return configuration;
    }

    public async Task<Dictionary<string, string>> GetConfigurationForService(ServiceType service)
    {
        var configurations = _ctx.ServiceConfiguration.Where(c => c.ServiceType == service)
            .ToList();
        var configurationDictionary = configurations.ToDictionary(c => c.Key, c => c.Value);

        return configurationDictionary;
    }

    public async Task<Dictionary<string, string>> GetConfiguration(ServiceType type)
    {
        return await _ctx.ServiceConfiguration
            .Where(config => config.ServiceType == type)
            .ToDictionaryAsync(config => config.Key, config => config.Value);
    }
}