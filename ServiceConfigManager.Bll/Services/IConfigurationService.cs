using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Models.Rabbit;
using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Bll.Services;

public interface IConfigurationService
{
    Guid AddConfigurationForService(AddConfigurationForServiceRequest request);
    void SendConfigurationToRabbit(ServiceConfigurationDto config);
}