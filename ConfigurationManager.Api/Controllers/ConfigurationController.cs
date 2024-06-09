using ConfigurationManager.Bll.Services;
using ConfigurationManager.Core.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConfigurationManager.Api.Controllers;

[ApiController]
[Route("/api/configuration/")]
public class ConfigurationController:Controller
{
    private readonly IConfigurationService _configurationService;
    private readonly Serilog.ILogger _logger = Log.ForContext<ConfigurationController>();

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    
    [HttpPost("create")]
    public ActionResult<Guid> AddConfigurationForService(AddConfigurationForServiceRequest request)
    {
        return Ok( _configurationService.AddConfigurationForService(request));
    }
}