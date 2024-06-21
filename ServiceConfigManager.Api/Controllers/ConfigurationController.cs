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
    
    [HttpPost]
    public async Task<ActionResult> AddConfigurationForService([FromBody]AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Получили запрос на создание конфигурации");
        await _configurationService.AddConfigurationForService(request);
        
        return Ok();
    }
}