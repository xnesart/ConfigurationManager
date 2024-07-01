using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.DataLayer.Repositories;

public interface IConfigurationRepository
{
    Task<Dictionary<string, string>> AddConfigurationForService(ServiceConfigurationDto newConfiguration);    
    Task<Dictionary<string, string>> UpdateConfigurationForService(ServiceConfigurationDto newConfiguration);
    Task<Dictionary<string, string>> GetConfiguration(ServiceType type);
    Task<Dictionary<string, string>> GetConfigurationForService(ServiceType service);
}