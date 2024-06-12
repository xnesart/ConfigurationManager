using System.Text;
using System.Text.Json;
using AutoMapper;
using RabbitMQ.Client;
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
    
    public ConfigurationService(IMapper mapper, IConfigurationRepository configurationRepository)
    {
        _mapper = mapper;
        _configurationRepository = configurationRepository;
    }
    
    public Guid AddConfigurationForService(AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Сервисы: добавление конфигурации: маппим");
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);
        _logger.Information($"Сервисы: добавление конфигурации: идем в метод репозитория");

        var res =_configurationRepository.AddConfigurationForService(newConfig);
        
        _logger.Information($"Сервисы: добавление конфигурации: отправляем новую конфигурацию в рэббит");
        SendConfigurationToRabbit(newConfig);
        
        return res;
    }

    public void SendConfigurationToRabbit(ServiceConfigurationDto newConfiguration)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "configuration_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = JsonSerializer.Serialize(newConfiguration);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
            routingKey: "configuration_queue",
            basicProperties: null,
            body: body);

        _logger.Information($"[x] Sent {message} in Rabbit");
    }
}