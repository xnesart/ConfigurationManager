using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.DataLayer.Repositories;

public interface IConfigurationRepository
{
     Task<Dictionary<string,string>> AddConfigurationForService(ServiceConfigurationDto newConfiguration);
     Task<Dictionary<string, string>> GetConfiguration(ServiceType type);
}