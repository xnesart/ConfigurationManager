using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.Core.Models.Rabbit;

public class SettingsModel
{
    public ServiceType ServiceType { get; set; }
    public Dictionary<string,string> Settigns { get; set; }
}