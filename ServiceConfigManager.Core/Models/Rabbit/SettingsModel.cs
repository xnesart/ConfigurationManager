using ServiceConfigManager.Core.Enums;

namespace Messaging.Shared;

public class SettingsModel
{
    public ServiceType ServiceType { get; set; }
    public Dictionary<string,string> Settings { get; set; }
}