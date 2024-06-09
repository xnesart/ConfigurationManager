using AutoMapper;
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

        return _configurationRepository.AddConfigurationForService(newConfig);
    }
}