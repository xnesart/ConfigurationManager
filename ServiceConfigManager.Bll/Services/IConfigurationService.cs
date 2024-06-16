using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Bll.Services;

public interface IConfigurationService
{
    Task<Guid> AddConfigurationForService(AddConfigurationForServiceRequest request);
}