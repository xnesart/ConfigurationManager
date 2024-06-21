using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Bll.Services;

public interface IConfigurationService
{
    Task AddConfigurationForService(AddConfigurationForServiceRequest request);
}