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
        _logger.Information($"Получили запрос на изменение конфигурации  {request.ServiceType}, {request.Key}, {request.Value}");
        await _configurationService.UpdateConfigurationForService(request);
        
        return Ok();
    }

    [HttpDelete]
    public async Task DeleteConfigurationForService([FromBody] AddConfigurationForServiceRequest request)
    {
        _logger.Information($"Получили запрос на удаление конфигурации  {request.ServiceType}, {request.Key}, {request.Value}");
        
        await _configurationService.DeleteConfigurationForService(request);
    }
    
    [HttpGet]
    public async Task<ActionResult<Dictionary<string,string>>> ConfigurationForService(ServiceType service)
    {
        _logger.Information($"Получили запрос на получение конфигурации {service}");
        var res = await _configurationService.GetConfigurationForService(service);
        
        return Ok(res);
    }
}