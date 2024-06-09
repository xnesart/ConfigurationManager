using ServiceConfigManager.Core;
using ServiceConfigManager.Core.DTOs;

namespace ServiceConfigManager.DataLayer.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    protected readonly DataBaseContext _ctx;
    
    public ConfigurationRepository(DataBaseContext ctx)
    {
        _ctx = ctx;
    }

    public Guid AddConfigurationForService(ServiceConfigurationDto newConfiguration)
    {
        _ctx.Add(newConfiguration);
        _ctx.SaveChanges();
        
        return newConfiguration.Id;
    }
}