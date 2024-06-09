using ConfigurationManager.Core.DTOs;

namespace ConfigurationManager.DataLayer.Repositories;

public interface IConfigurationRepository
{
    Guid AddConfigurationForService(ServiceConfigurationDto newConfiguration);
}