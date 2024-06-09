using ConfigurationManager.Core.Models.Requests;

namespace ConfigurationManager.Bll.Services;

public interface IConfigurationService
{
    Guid AddConfigurationForService(AddConfigurationForServiceRequest request);
}