using ServiceConfigManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConfigManager.Core.Models.Requests;

public class UpdateConfigurationForServiceRequest
{
    public ServiceType ServiceType { get; set; }
    public string Key { get; set; }
    public string NewValue { get; set; }
}
