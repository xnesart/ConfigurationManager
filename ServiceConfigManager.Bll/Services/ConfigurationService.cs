using AutoMapper;
using MassTransit;
using Messaging.Shared;
using Serilog;
using ServiceConfigManager.Core.DTOs;
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

    public async Task<Guid> AddConfigurationForService(AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Сервисы: добавление конфигурации: маппим");
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);
        _logger.Information($"Сервисы: добавление конфигурации: идем в метод репозитория");

        var res = await _configurationRepository.AddConfigurationForService(newConfig);

        _logger.Information($"Сервисы: добавление конфигурации: отправляем новую конфигурацию в рэббит");
        await SendConfigurationToRabbit(newConfig);

        return res;
    }

    // private async Task SendConfigurationToRabbit(ServiceConfigurationDto config)
    // {
    //     var newSettings = ProcessingToSettingsModel(config);
    //
    //     var factory = new ConnectionFactory() { HostName = "localhost" };
    //     using var connection = factory.CreateConnection();
    //     using var channel = connection.CreateModel();
    //
    //     channel.QueueDeclare(queue: "configuration_queue",
    //         durable: false,
    //         exclusive: false,
    //         autoDelete: false,
    //         arguments: null);
    //
    //     var message = JsonSerializer.Serialize(newSettings);
    //     var body = Encoding.UTF8.GetBytes(message);
    //
    //     await Task.Run(() =>
    //     {
    //         channel.BasicPublish(exchange: "",
    //             routingKey: "configuration_queue",
    //             basicProperties: null,
    //             body: body);
    //
    //         _logger.Information($"{message} отправлен в RabbitMq");
    //     });
    // } 
    private async Task SendConfigurationToRabbit(ServiceConfigurationDto config)
    {
        var newSettings = ProcessingToSettingsModel(config);

        // Отправка сообщения через MassTransit
        await _publishEndpoint.Publish(newSettings);

        _logger.Information($"Сервисы: добавление конфигурации: {newSettings} отправлено в RabbitMQ");
    }

    private SettingsModel ProcessingToSettingsModel(ServiceConfigurationDto config)
    {
        _logger.Information($"Сервисы: отправление конфигурации в рэббит: маппим");

        return _mapper.Map<SettingsModel>(config);
    }
}