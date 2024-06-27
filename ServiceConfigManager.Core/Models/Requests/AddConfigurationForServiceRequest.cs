using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.Core.Models.Requests;

public class AddConfigurationForServiceRequest
{
    public ServiceType ServiceType { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}