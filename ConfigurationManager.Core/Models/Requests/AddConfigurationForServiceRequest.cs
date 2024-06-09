namespace ConfigurationManager.Core.Models.Requests;

public class AddConfigurationForServiceRequest
{
    public string Service { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}