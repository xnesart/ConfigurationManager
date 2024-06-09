using ServiceConfigManager.Bll.Services;
using ServiceConfigManager.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;

using Serilog;
using ILogger = Serilog.ILogger;

namespace ServiceConfigManager.Api.Controllers;

[ApiController]
[Route("/api/configuration")]
public class ConfigurationController:Controller
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger _logger = Log.ForContext<ConfigurationController>();

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    
    [HttpPost("/add")]
    public ActionResult<Guid> AddConfigurationForService([FromBody]AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Получили запрос на создание конфигурации");
        return Ok( _configurationService.AddConfigurationForService(request));
    }
}