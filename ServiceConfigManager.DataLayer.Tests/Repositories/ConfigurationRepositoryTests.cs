using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using ServiceConfigManager.Core;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;
using ServiceConfigManager.Core.Exceptions;
using ServiceConfigManager.DataLayer.Repositories;

namespace ServiceConfigManager.DataLayer.Tests.Repositories
{
    public class ConfigurationRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _dbContextOptions;
        private readonly Mock<DataBaseContext> _ctxMock;
        private readonly IConfigurationRepository _sut;

        public ConfigurationRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: "ConfigurationDatabase")
                .Options;

            _ctxMock = new Mock<DataBaseContext>(_dbContextOptions);
            _sut = new ConfigurationRepository(_ctxMock.Object);
        }

        [Fact]
        public async Task AddConfigurationForService_CorrectDtoReceived_DictionaryReturned()
        {
            // Arrange
            var newConfiguration = new ServiceConfigurationDto
            {
                ServiceType = ServiceType.crm,
                Key = "connectionString",
                Value = "someValue"
            };
            
            var mockData = new List<ServiceConfigurationDto>
            {
                new ServiceConfigurationDto
                    { ServiceType = ServiceType.crm, Key = "connectionString", Value = "someValue" }
            };

            _ctxMock.Setup(x => x.ServiceConfiguration)
                .ReturnsDbSet(mockData);

            // Act
            var result = await _sut.AddConfigurationForService(newConfiguration);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("connectionString"));
            Assert.Equal("someValue", result["connectionString"]);
        }

        [Fact]
        public async Task UpdateConfigurationForService_ExistingConfigurationUpdated_DictionaryReturned()
        {
            // Arrange
            var existingConfiguration = new ServiceConfigurationDto
            {
                ServiceType = ServiceType.crm,
                Key = "connectionString",
                Value = "oldValue"
            };
            
            _ctxMock.Setup(x => x.ServiceConfiguration)
                .ReturnsDbSet(new[] { existingConfiguration });

            var newConfiguration = new ServiceConfigurationDto
            {
                ServiceType = ServiceType.crm,
                Key = "connectionString",
                Value = "newValue"
            };

            // Act
            var result = await _sut.UpdateConfigurationForService(newConfiguration);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("connectionString"));
        }
        
        [Fact]
        public async Task UpdateConfigurationForService_ConfigurationNotFound_ThrowsException()
        {
            // Arrange
            var nonExistingConfiguration = new ServiceConfigurationDto
            {
                ServiceType = ServiceType.crm,
                Key = "nonExistingKey",
                Value = "value"
            };

            _ctxMock.Setup(x => x.ServiceConfiguration)
                .ReturnsDbSet(new List<ServiceConfigurationDto>());

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _sut.UpdateConfigurationForService(nonExistingConfiguration));
        }

        [Fact]
        public async Task GetConfigurationForService_ExistingServiceType_ConfigurationReturned()
        {
            // Arrange
            var existingConfiguration = new ServiceConfigurationDto
            {
                ServiceType = ServiceType.crm,
                Key = "connectionString",
                Value = "someValue"
            };

            _ctxMock.Setup(x => x.ServiceConfiguration)
                .ReturnsDbSet(new[] { existingConfiguration });

            // Act
            var result = await _sut.GetConfigurationForService(ServiceType.crm);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("connectionString"));
            Assert.Equal("someValue", result["connectionString"]);
        }

        [Fact]
        public async Task GetConfiguration_ConfigurationNotFound_ReturnsEmptyDictionary()
        {
            // Arrange
            _ctxMock.Setup(x => x.ServiceConfiguration)
                .ReturnsDbSet(new List<ServiceConfigurationDto>());

            // Act
            var result = await _sut.GetConfiguration(ServiceType.crm);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}