using AutoMapper;
using MassTransit;
using Messaging.Shared;
using Moq;
using ServiceConfigManager.Bll.Services;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Models.Requests;
using ServiceConfigManager.DataLayer.Repositories;

namespace ServiceConfigManager.Bll.Tests.Services;

public class ConfigurationServiceTests
{
    private readonly Mock<IConfigurationRepository> _configurationRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly IConfigurationService _sut;

    public ConfigurationServiceTests()
    {
        _configurationRepositoryMock = new Mock<IConfigurationRepository>();
        _mapperMock = new Mock<IMapper>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _sut = new ConfigurationService(_mapperMock.Object, _configurationRepositoryMock.Object,
            _publishEndpointMock.Object);
    }

    [Fact]
    public async Task AddConfigurationForService_ValidRequestSent_PublishConfigurationMessage()
    {
        //arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = ServiceType.crm,
            Key = "someKey",
            Value = "someValue"
        };

        var returnFromRepo = new Dictionary<string, string>()
        {
            { "someKey", "someValue" },
        };

        var mappedConfiguration = new ServiceConfigurationDto();

        _configurationRepositoryMock.Setup(repo =>
            repo.AddConfigurationForService(It.IsAny<ServiceConfigurationDto>())).ReturnsAsync(returnFromRepo);
        _mapperMock.Setup(m => m.Map<ServiceConfigurationDto>(request)).Returns(mappedConfiguration);

        //act
        var actual = _sut.AddConfigurationForService(request);

        //assert
        _configurationRepositoryMock.Verify(
            repository => repository.AddConfigurationForService(mappedConfiguration), Times.Once);
        _publishEndpointMock.Verify(ep => ep.Publish(It.Is<ConfigurationMessage>(m =>
            m.Configurations == returnFromRepo && m.ServiceType == request.ServiceType), default), Times.Once);
    }

    [Fact]
    public async Task AddConfigurationForService_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        AddConfigurationForServiceRequest request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddConfigurationForService(request));
    }

    [Fact]
    public async Task AddConfigurationForService_MappingError_ThrowsMappingException()
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = ServiceType.crm,
            Key = "someKey",
            Value = "someValue"
        };

        _mapperMock.Setup(m => m.Map<ServiceConfigurationDto>(request)).Throws(new Exception("Mapping error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddConfigurationForService(request));
    }

    [Fact]
    public async Task AddConfigurationForService_RabbitMqError_ThrowsRabbitMqException()
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = ServiceType.crm,
            Key = "someKey",
            Value = "someValue"
        };

        var mappedConfiguration = new ServiceConfigurationDto();
        var returnFromRepo = new Dictionary<string, string>()
        {
            { "someKey", "someValue" },
        };

        _mapperMock.Setup(m => m.Map<ServiceConfigurationDto>(request)).Returns(mappedConfiguration);
        _configurationRepositoryMock.Setup(repo => repo.AddConfigurationForService(mappedConfiguration))
            .ReturnsAsync(returnFromRepo);
        _publishEndpointMock.Setup(ep => ep.Publish(It.IsAny<ConfigurationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("RabbitMQ error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddConfigurationForService(request));
    }

    [Fact]
    public async Task GetConfigurationForService_ValidRequestSent_PublishConfigurationMessage()
    {
        //arrange
        var type = ServiceType.crm;
        var expected = new Dictionary<string, string>()
        {
            { "key", "value" }
        };

        _configurationRepositoryMock.Setup(repo =>
            repo.GetConfigurationForService(It.IsAny<ServiceType>())).ReturnsAsync(expected);

        //act
        var result = await _sut.GetConfigurationForService(type);

        //assert
        _configurationRepositoryMock.Verify(repository => repository.GetConfigurationForService(type), Times.Once);
    }

    [Fact]
    public async Task GetConfigurationForService_NoConfigurationFound_ReturnsEmptyDictionary()
    {
        // Arrange
        var type = ServiceType.crm;
        _configurationRepositoryMock.Setup(repo =>
            repo.GetConfigurationForService(type)).ReturnsAsync(new Dictionary<string, string>());

        // Act
        var result = await _sut.GetConfigurationForService(type);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _configurationRepositoryMock.Verify(repository => repository.GetConfigurationForService(type), Times.Once);
    }
    
    [Fact]
    public async Task UpdateConfigurationForService_ValidRequest_PublishConfigurationMessage()
    {
        //arrange
        var request = new AddConfigurationForServiceRequest()
        {
            ServiceType = ServiceType.crm,
            Key = "someKey",
            Value = "someValue"
        };
        var returnFromRepo = new Dictionary<string, string>()
        {
            { "someKey", "someValue" },
        };
        var mappedConfiguration = new ServiceConfigurationDto();

        _configurationRepositoryMock.Setup(repo =>
            repo.UpdateConfigurationForService(It.IsAny<ServiceConfigurationDto>())).ReturnsAsync(returnFromRepo);
        _configurationRepositoryMock.Setup(repo =>
            repo.AddConfigurationForService(It.IsAny<ServiceConfigurationDto>())).ReturnsAsync(returnFromRepo);
        _mapperMock.Setup(m => m.Map<ServiceConfigurationDto>(request)).Returns(mappedConfiguration);

        //act
        var actual = _sut.UpdateConfigurationForService(request);

        //assert
        _configurationRepositoryMock.Verify(repository => repository.UpdateConfigurationForService(mappedConfiguration),
            Times.Once);
        _publishEndpointMock.Verify(ep => ep.Publish(It.Is<ConfigurationMessage>(m =>
            m.Configurations == returnFromRepo && m.ServiceType == request.ServiceType), default), Times.Once);
    }

    [Fact]
    public async Task UpdateConfigurationForService_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        AddConfigurationForServiceRequest request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateConfigurationForService(request));
    }

    [Fact]
    public async Task UpdateConfigurationForService_MappingThrowsException_ThrowsException()
    {
        // Arrange
        var request = new AddConfigurationForServiceRequest
        {
            ServiceType = ServiceType.crm,
            Key = "someKey",
            Value = "someValue"
        };

        _mapperMock.Setup(m => m.Map<ServiceConfigurationDto>(request)).Throws(new InvalidOperationException());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateConfigurationForService(request));
    }
}