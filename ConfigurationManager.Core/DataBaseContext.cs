using ConfigurationManager.Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManager.Core;

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