using ServiceConfigManager.Core.DTOs;

namespace ServiceConfigManager.DataLayer.Repositories;

public interface IConfigurationRepository
{
     Task<Guid> AddConfigurationForService(ServiceConfigurationDto newConfiguration);
}