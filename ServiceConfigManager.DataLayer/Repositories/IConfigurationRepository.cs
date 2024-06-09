using ServiceConfigManager.Core.DTOs;

namespace ServiceConfigManager.DataLayer.Repositories;

public interface IConfigurationRepository
{
    Guid AddConfigurationForService(ServiceConfigurationDto newConfiguration);
}