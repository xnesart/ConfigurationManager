using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Bll.Services;

public interface IConfigurationService
{
    Task AddConfigurationForService(AddConfigurationForServiceRequest request);
    Task<Dictionary<string,string>> GetConfigurationForService(ServiceType service);
    Task UpdateConfigurationForService(AddConfigurationForServiceRequest request);
}