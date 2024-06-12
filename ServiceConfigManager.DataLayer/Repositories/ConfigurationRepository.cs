using Microsoft.EntityFrameworkCore;
using Serilog;
using ServiceConfigManager.Core;
using ServiceConfigManager.Core.DTOs;
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

    public async Task<Guid> AddConfigurationForService(ServiceConfigurationDto newConfiguration)
    {
        _logger.Information($"Сервисы: добавление конфигурации: добавляем новую конфигурацию в контекст");

        await _ctx.AddAsync(newConfiguration);
        await _ctx.SaveChangesAsync();

        _logger.Information($"Возвращаем айди конфигурации {newConfiguration.Id}");

        return newConfiguration.Id;
    }
    public async Task UpdateConfigurationForService(ServiceConfigurationDto updatedConfiguration)
    {
        _logger.Information($"Сервисы: обновление конфигурации: обновляем конфигурацию в контексте");
        var existingConfiguration = await _ctx.ServiceConfiguration.FirstOrDefaultAsync(c => c.Id == updatedConfiguration.Id);

        if (existingConfiguration != null)
        {
            existingConfiguration.Value = updatedConfiguration.Value;
            existingConfiguration.Key = updatedConfiguration.Key;
            existingConfiguration.ServiceType = updatedConfiguration.ServiceType;

            _ctx.Update(existingConfiguration);
            await _ctx.SaveChangesAsync();

            _logger.Information($"Конфигурация успешно обновлена");
        }
        else
        {
            _logger.Error($"Конфигурация с Id {updatedConfiguration.Id} не найдена");
            throw new NotFoundException($"Конфигурация с Id {updatedConfiguration.Id} не найдена");
        }
    }

}