using Microsoft.EntityFrameworkCore;
using Serilog;
using ServiceConfigManager.Core;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Exceptions;

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

        var configuration = await GetConfigurationForService(newConfiguration.ServiceType);
        return configuration;
    }

    public async Task<Dictionary<string, string>> UpdateConfigurationForService(
        ServiceConfigurationDto newConfiguration)
    {
        var existingConfig = await _ctx.ServiceConfiguration
            .FirstOrDefaultAsync(c => c.ServiceType == newConfiguration.ServiceType &&
                                      c.Key == newConfiguration.Key);

        if (existingConfig == null)
        {
            _logger.Error("configuration not found, exception");
            throw new NotFoundException("конфигурация не найдена");
        }

        _logger.Information("added config to context");
        _ctx.ServiceConfiguration.Remove(existingConfig);
        _ctx.ServiceConfiguration.Add(newConfiguration);

        await _ctx.SaveChangesAsync();

        var updatedConfigurations = await _ctx.ServiceConfiguration
            .Where(c => c.ServiceType == newConfiguration.ServiceType)
            .ToListAsync();

        var configDictionary = updatedConfigurations.ToDictionary(c => c.Key, c => c.Value);

        return configDictionary;
    }
    
    public async Task<Dictionary<string, string>> GetConfigurationForService(ServiceType service)
    {
        var configurations = _ctx.ServiceConfiguration.Where(c => c.ServiceType == service)
            .ToList();
        var configurationDictionary = configurations.ToDictionary(c => c.Key, c => c.Value);

        return configurationDictionary;
    }

    public async Task<Dictionary<string, string>> DeleteConfigurationForService(
        ServiceConfigurationDto configurationForDelete)
    {
        var serviceType = configurationForDelete.ServiceType;

        var configuration = await _ctx.ServiceConfiguration.Where(c =>
            c.ServiceType == configurationForDelete.ServiceType && c.Key == configurationForDelete.Key &&
            c.Value == configurationForDelete.Value).FirstOrDefaultAsync();

        if (configuration != null)
        {
            _ctx.ServiceConfiguration.Remove(configuration);
            await _ctx.SaveChangesAsync();
        }

        return await GetConfigurationForService(serviceType);
    }
}