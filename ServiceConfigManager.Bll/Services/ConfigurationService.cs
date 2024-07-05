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

    public ConfigurationService(IMapper mapper, IConfigurationRepository configurationRepository,
        IPublishEndpoint publishEndpoint)
    {
        _mapper = mapper;
        _configurationRepository = configurationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task AddConfigurationForService(AddConfigurationForServiceRequest request)
    {
        _logger.Information("Сервисы: добавление конфигурации: маппим");
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);

        _logger.Information("Сервисы: добавление конфигурации: идем в метод репозитория");
        var res = await _configurationRepository.AddConfigurationForService(newConfig);

        _logger.Information($"Сервисы: добавление конфигурации: отправляем новую конфигурацию в RabbitMQ для сервиса {request.ServiceType}");

        var message = new ConfigurationMessage()
        {
            Configurations = res,
            ServiceType = request.ServiceType
        };
        
        await SendConfigurationToRabbit(message);
    }

    public async Task<Dictionary<string, string>> GetConfigurationForService(ServiceType service)
    {
        _logger.Information($"Сервисы: добавление конфигурации: идем в метод репозитория");

        var res = await _configurationRepository.GetConfigurationForService(service);

        return res;
    }

    public async Task UpdateConfigurationForService(AddConfigurationForServiceRequest request)
    {
        _logger.Information(
            $"Сервисы: изменение конфигурации: маппим{request.ServiceType}, {request.Value}, {request.Key}");
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);
        _logger.Information($"Сервисы: изменение конфигурации: идем в метод репозитория");

        var res = await _configurationRepository.UpdateConfigurationForService(newConfig);

        var message = new ConfigurationMessage()
        {
            Configurations = res,
            ServiceType = request.ServiceType
        };

        _logger.Information($"Сервисы: изменение конфигурации: отправляем новую конфигурацию в рэббит");
        await SendConfigurationToRabbit(message);
    }

    private async Task SendConfigurationToRabbit(ConfigurationMessage configurationMessage)
    {
        if (configurationMessage == null)
        {
            _logger.Information("Сервисы: отправление конфигурации в RabbitMQ: конфигурация пуста, отправка отменена");
            return;
        }

        _logger.Information(
            "Сервисы: отправление конфигурации в RabbitMQ: маппим. Количество ключей: {KeyCount}, Ключи: {Keys}",
            configurationMessage.Configurations.Count, string.Join(", ", configurationMessage.Configurations.Keys));
        
        await _publishEndpoint.Publish(configurationMessage);

        _logger.Information($"Сервисы: добавление конфигурации: конфигурация отправлена в RabbitMQ{configurationMessage.Configurations} для сервиса {configurationMessage.ServiceType}");
    }
}