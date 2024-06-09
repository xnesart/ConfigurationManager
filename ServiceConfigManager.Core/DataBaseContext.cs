using Microsoft.EntityFrameworkCore;
using ServiceConfigManager.Core.DTOs;

namespace ServiceConfigManager.Core;

public class DataBaseContext:DbContext
{
    public virtual DbSet<ServiceConfigurationDto> ServiceConfiguration { get; set; }

    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
    }

    public DataBaseContext()
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}