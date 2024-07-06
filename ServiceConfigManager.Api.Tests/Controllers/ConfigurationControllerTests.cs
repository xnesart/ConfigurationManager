using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceConfigManager.Api.Controllers;
using ServiceConfigManager.Bll.Services;
using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Api.Tests.Controllers;

public class ConfigurationControllerTests
{
    private readonly ConfigurationController _sut;
    private readonly Mock<IConfigurationService> _configurationService;

    public ConfigurationControllerTests()
    {
        _configurationService = new Mock<IConfigurationService>();
        _sut = new ConfigurationController(_configurationService.Object);
    }

    [Theory]
    [InlineData(ServiceType.crm, "connectionString", "server=194.87.210.5;Port=5432;")]
    [InlineData(ServiceType.unknown, "string", "string1")]
    public async Task AddConfigurationForService_CorrectRequestSent_AllOkReturned(ServiceType serviceType, string key, string value)
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
    [InlineData(ServiceType.crm, "connectionString", "someString")]
    public async Task ConfigurationForServicePatсh_CorrectRequestSent_AllOkReturned(ServiceType serviceType, string key,
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

    [Fact]
    public async Task ConfigurationForServiceGet_CorrectRequestSent_DictionaryReturned()
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
}