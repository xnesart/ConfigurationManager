using ServiceConfigManager.Bll.Services;
using ServiceConfigManager.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;

using Serilog;
using ServiceConfigManager.Core.Enums;
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
        _logger.Information($"Получили запрос на создание конфигурации {request.ServiceType}, {request.Key}, {request.Value}");
        await _configurationService.AddConfigurationForService(request);
        
        return Ok();
    }
    
    [HttpPatch]
    public async Task<ActionResult> ConfigurationForService([FromBody]AddConfigurationForServiceRequest request)
    {
        if (request.ServiceType == null || request.Value == null || request.Key == null)
        {
            _logger.Error("Received a request with invalid data. Check request fields.");
            return BadRequest("Invalid request data. ServiceType, Key, and Value cannot be null.");
        }

        _logger.Information($"Received a request to update configuration {request.ServiceType}, {request.Key}, {request.Value}");
        await _configurationService.UpdateConfigurationForService(request);

        return Ok();
    }

    
    [HttpGet]
    public async Task<ActionResult<Dictionary<string,string>>> ConfigurationForService(ServiceType service)
    {
        _logger.Information($"Получили запрос на получение конфигурации {service}");
        var res = await _configurationService.GetConfigurationForService(service);
        
        return Ok(res);
    }
}