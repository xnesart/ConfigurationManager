using ServiceConfigManager.Bll.Services;
using ServiceConfigManager.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ServiceConfigManager.Api.Controllers;

[ApiController]
[Route("/api/configuration")]
public class ConfigurationController:Controller
{
    private readonly IConfigurationService _configurationService;
    //private readonly Serilog.ILogger _logger = Log.ForContext<ConfigurationController>();

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    
    [HttpPost("/create")]
    public ActionResult<Guid> AddConfigurationForService([FromBody]AddConfigurationForServiceRequest request)
    {
        return Ok( _configurationService.AddConfigurationForService(request));
    }
}