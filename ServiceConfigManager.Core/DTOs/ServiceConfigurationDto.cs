using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.Core.DTOs;

public class ServiceConfigurationDto
{
    public Guid Id { get; set; }
    public ServiceType ServiceType { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}