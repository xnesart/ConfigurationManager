using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Models.Rabbit;
using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Bll.Services;

public interface IConfigurationService
{
    Task<Guid> AddConfigurationForService(AddConfigurationForServiceRequest request);
    Task UpdateConfigurationForService(UpdateConfigurationForServiceRequest request);
}