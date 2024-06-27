using Microsoft.EntityFrameworkCore;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Enums;

namespace ServiceConfigManager.Core;

public class DataBaseContext : DbContext
{
    public virtual DbSet<ServiceConfigurationDto> ServiceConfiguration { get; set; }

    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<ServiceType>();

        modelBuilder.Entity<ServiceConfigurationDto>()
            .Property(e => e.ServiceType)
            .HasConversion<string>();
    }
}