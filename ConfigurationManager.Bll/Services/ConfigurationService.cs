using AutoMapper;
using ConfigurationManager.Core.DTOs;
using ConfigurationManager.Core.Models.Requests;
using ConfigurationManager.DataLayer.Repositories;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;


namespace ConfigurationManager.Bll.Services;

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