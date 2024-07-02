using AutoMapper;
using MassTransit;
using Messaging.Shared;
using Serilog;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Models.Requests;
using ServiceConfigManager.DataLayer.Repositories;


namespace ServiceConfigManager.Bll.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly ILogger _logger = Log.ForContext<ConfigurationService>();
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public ConfigurationService(IMapper mapper, IConfigurationRepository configurationRepository, IPublishEndpoint publishEndpoint)
    {
        _mapper = mapper;
        _configurationRepository = configurationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task AddConfigurationForService(AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Сервисы: добавление конфигурации: маппим");
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);
        _logger.Information($"Сервисы: добавление конфигурации: идем в метод репозитория");

        var res = await _configurationRepository.AddConfigurationForService(newConfig);

        _logger.Information($"Сервисы: добавление конфигурации: отправляем новую конфигурацию в рэббит");
        await SendConfigurationToRabbit(res);
    } 
    
    public async Task<Dictionary<string, string>> GetConfigurationForService(ServiceType service)
    {
        _logger.Information($"Сервисы: добавление конфигурации: идем в метод репозитория");

        var res = await _configurationRepository.GetConfigurationForService(service);

        _logger.Information($"Сервисы: добавление конфигурации: отправляем новую конфигурацию в рэббит");
        await SendConfigurationToRabbit(res);
        
        return res;
    } 
    
    public async Task UpdateConfigurationForService(AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Сервисы: изменение конфигурации: маппим");
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);
        _logger.Information($"Сервисы: изменение конфигурации: идем в метод репозитория");

        var res = await _configurationRepository.UpdateConfigurationForService(newConfig);

        _logger.Information($"Сервисы: изменение конфигурации: отправляем новую конфигурацию в рэббит");
        await SendConfigurationToRabbit(res);
    }
    
    
    
    private async Task SendConfigurationToRabbit(Dictionary<string,string> config)
    {
        if (config == null || !config.Any())
        {
            _logger.Information("Сервисы: отправление конфигурации в RabbitMQ: конфигурация пуста, отправка отменена");
            return;
        }

        _logger.Information("Сервисы: отправление конфигурации в RabbitMQ: маппим");
  
        var message = new ConfigurationMessage
        {
            Configurations = config
        };

        // Отправка сообщения через MassTransit
        await _publishEndpoint.Publish(message);

        _logger.Information("Сервисы: добавление конфигурации: конфигурация отправлена в RabbitMQ");
    }
}