using AutoMapper;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Models.Requests;
using ServiceConfigManager.DataLayer.Repositories;



namespace ServiceConfigManager.Bll.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfigurationRepository _configurationRepository;
    // private readonly ILogger _logger;
    private readonly IMapper _mapper;
    
    public ConfigurationService(IMapper mapper, IConfigurationRepository configurationRepository)
    {
        _mapper = mapper;
        _configurationRepository = configurationRepository;
        // _logger = logger;
    }
    
    public Guid AddConfigurationForService(AddConfigurationForServiceRequest request)
    {
        var newConfig = _mapper.Map<ServiceConfigurationDto>(request);
        
        return _configurationRepository.AddConfigurationForService(newConfig);
    }
}