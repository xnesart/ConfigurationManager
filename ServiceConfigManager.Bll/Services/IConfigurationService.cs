using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Bll.Services;

public interface IConfigurationService
{
    Guid AddConfigurationForService(AddConfigurationForServiceRequest request);
}