namespace ConfigurationManager.Core.DTOs;

public class ServiceConfigurationDto
{
    public Guid Id { get; set; }
    public string Service { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}