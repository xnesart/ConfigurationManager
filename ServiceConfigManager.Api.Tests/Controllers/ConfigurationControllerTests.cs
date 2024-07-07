using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ServiceConfigManager.Api.Configuration;
using ServiceConfigManager.Api.Controllers;
using ServiceConfigManager.Bll.Services;
using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Exceptions;
using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Api.Tests.Controllers;

public class ConfigurationControllerTests
{
    private readonly ConfigurationController _sut;
    private readonly Mock<IConfigurationService> _configurationService;
    private readonly Mock<ILogger<ConfigurationController>> _loggerMock;

    public ConfigurationControllerTests()
    {
        _loggerMock = new Mock<ILogger<ConfigurationController>>();
        _configurationService = new Mock<IConfigurationService>();
        _sut = new ConfigurationController(_configurationService.Object);
    }

    [Theory]
    [InlineData(ServiceType.crm, "connectionString", "server=194.87.210.5;Port=5432;")]
    [InlineData(ServiceType.unknown, "string", "string1")]
    public async Task AddConfigurationForService_CorrectRequestSent_AllOkReturned(ServiceType serviceType, string key,
        string value)
    {
        //arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = serviceType,
            Key = key,
            Value = value
        };
        _configurationService.Setup(service => service.AddConfigurationForService(request)).Returns(Task.CompletedTask);

        //act
        var actual = await _sut.AddConfigurationForService(request);

        //assert
        _configurationService.Verify(service => service.AddConfigurationForService(request), Times.Once);
        var actionResult = Assert.IsType<OkResult>(actual);
        Assert.Equal(200, actionResult.StatusCode);
    }

    [Theory]
    [InlineData(null, null, "someValue")]
    public async Task ConfigurationForService_InvalidRequest_ReturnsBadRequest(ServiceType serviceType, string key,
        string value)
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = serviceType,
            Key = key,
            Value = value
        };

        // Act
        var result = await _sut.ConfigurationForService(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task AddConfigurationForService_ValidationException_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest
        {
            ServiceType = ServiceType.crm,
            Key = "connectionString",
            Value = "someConnectionString"
        };

        var middleware = new ExceptionMiddleware(next: (innerHttpContext) =>
        {
            throw new ValidationException("Validation failed");
        });

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(responseBody);

        Assert.NotNull(errorDetails);
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, errorDetails.StatusCode);
        Assert.Equal("Validation failed", errorDetails.Message);
    }

    [Theory]
    [InlineData(ServiceType.crm, "connectionString", "someString")]
    public async Task PatchConfigurationForService_CorrectRequestSent_AllOkReturned(ServiceType serviceType, string key,
        string value)
    {
        //arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = serviceType,
            Key = key,
            Value = value
        };

        _configurationService.Setup(service => service.UpdateConfigurationForService(request))
            .Returns(Task.CompletedTask);

        //act
        var actual = await _sut.ConfigurationForService(request);

        //assert
        _configurationService.Verify(service => service.UpdateConfigurationForService(request), Times.Once);
        var actionResult = Assert.IsType<OkResult>(actual);
        Assert.Equal(200, actionResult.StatusCode);
    }
    
    [Theory]
    [InlineData(ServiceType.crm, "connectionString", "someString")]
    public async Task PatchConfigurationForService_ServiceNotFound_ThrowsNotFoundException(ServiceType serviceType, string key, string value)
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest
        {
            ServiceType = serviceType,
            Key = key,
            Value = value
        };

        _configurationService.Setup(service => service.UpdateConfigurationForService(request))
            .ThrowsAsync(new NotFoundException("Configuration not found"));

        // Act
        Func<Task> act = () => _sut.ConfigurationForService(request);

        // Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Equal("Configuration not found", exception.Message);
    }
    
    [Theory]
    [InlineData(ServiceType.crm, "connectionString", "someString")]
    public async Task PatchConfigurationForService_UnexpectedError_ThrowsException(ServiceType serviceType, string key, string value)
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest
        {
            ServiceType = serviceType,
            Key = key,
            Value = value
        };

        _configurationService.Setup(service => service.UpdateConfigurationForService(request))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        Func<Task> act = () => _sut.ConfigurationForService(request);

        // Assert
        var exception = await Assert.ThrowsAsync<Exception>(act);
        Assert.Equal("Unexpected error", exception.Message);
    }

    [Fact]
    public async Task GetConfigurationForService_CorrectRequestSent_DictionaryReturned()
    {
        //arrange
        var request = ServiceType.crm;
        var expected = new Dictionary<string, string>()
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };
        _configurationService.Setup(service => service.GetConfigurationForService(request))
            .ReturnsAsync(expected);

        //act
        var actual = await _sut.ConfigurationForService(request);

        //assert
        _configurationService.Verify(service => service.GetConfigurationForService(request), Times.Once);
        var actionResult = Assert.IsType<ActionResult<Dictionary<string, string>>>(actual);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var okObjectResult = actionResult.Result as OkObjectResult;
        Assert.Equal(200, okObjectResult.StatusCode);
        Assert.Equal(expected, okObjectResult.Value);
    }
    
    [Fact]
    public async Task GetConfigurationForService_ServiceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var request = ServiceType.crm;
        _configurationService.Setup(service => service.GetConfigurationForService(request))
            .ThrowsAsync(new NotFoundException("Configuration not found"));

        // Act
        Func<Task> act = () => _sut.ConfigurationForService(request);

        // Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Equal("Configuration not found", exception.Message);
    }

    
    [Fact]
    public async Task GetConfigurationForService_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = ServiceType.crm;
        _configurationService.Setup(service => service.GetConfigurationForService(request))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        Func<Task> act = () => _sut.ConfigurationForService(request);

        // Assert
        var exception = await Assert.ThrowsAsync<Exception>(act);
        Assert.Equal("Unexpected error", exception.Message);
    }
}